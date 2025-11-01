using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Net.payOS.Types;
using Swashbuckle.AspNetCore.Annotations;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Services.ServiceProvider;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Produces("application/json")]
    public class PaymentController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IServiceProviders serviceProvider, ILogger<PaymentController> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Authorize("DeliveringStaffPolicy")]
        [SwaggerOperation(Summary = "Get all payments", Description = "Retrieves all payment records.")]
        [SwaggerResponse(200, "Payments retrieved successfully.")]
        [SwaggerResponse(401, "Unauthorized - Invalid token or insufficient permissions.")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _serviceProvider.UserSubscriptionService.GetAllSubscriptionsAsync();
            return Ok(payments);
        }

        [HttpPost("create-payment")]
        [Authorize]
        [SwaggerOperation(Summary = "Create payment", Description = "Creates a new payment link for subscription purchase.")]
        [SwaggerResponse(200, "Payment link created successfully.")]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            if (!ModelState.IsValid || request == null || request.UserId <= 0 || request.Amount <= 0)
            {
                _logger.LogWarning("Invalid model state for CreatePayment request.");
                return BadRequest(new { Message = "Invalid request data. UserId and Amount are required." });
            }

            try
            {
                // Create unique orderCode (used as PaymentGatewayTransactionId)
                long orderCode = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                // Create ItemData for PayOS
                var item = new ItemData(request.ItemName, request.Quantity, (int)request.Amount);
                var items = new List<ItemData> { item };

                // Configure PaymentData
                var paymentData = new PaymentData(
                    orderCode,
                    (int)request.Amount,
                    request.Description,
                    items,
                    "https://www.ezexam.online/login",
                    "https://www.ezexam.online/login"
                );

                // Create payment link
                CreatePaymentResult result = await _serviceProvider.PayOSService.CreatePaymentLink(paymentData);

                // Prepare UserSubscriptionRequest for AddSubscriptionAsync
                var subscriptionRequest = new UserSubscriptionRequest
                {
                    UserId = request.UserId,
                    SubscriptionTypeId = request.SubscriptionTypeId ?? 1,
                    EndDate = DateTime.UtcNow.AddMonths(1),
                    Amount = request.Amount,
                    PaymentGatewayTransactionId = orderCode.ToString()
                };

                // Save subscription information
                await _serviceProvider.UserSubscriptionService.AddSubscriptionAsync(subscriptionRequest);

                _logger.LogInformation("Payment link created successfully for user {UserId} with order code {OrderCode}", request.UserId, orderCode);
                return Ok(new { CheckoutUrl = result.checkoutUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment link for user {UserId}", request.UserId);
                return StatusCode(500, new { Message = "An error occurred while creating the payment link." });
            }
        }
        // GET: /api/payments/user-subscriptions/{userId}?onlyActive=true
        [HttpGet("user-subscriptions/{userId:int}")]
        [Authorize] // hoặc [AllowAnonymous] nếu bạn muốn public
        [SwaggerOperation(
            Summary = "Get user subscriptions",
            Description = "Lấy danh sách subscription của user theo UserId. Thêm ?onlyActive=true để chỉ lấy gói còn hạn & đã thanh toán."
        )]
        [SwaggerResponse(200, "Danh sách subscription")]
        [SwaggerResponse(404, "User không tồn tại hoặc không có subscription")]
        public async Task<IActionResult> GetUserSubscriptionsByUserId(
            [FromRoute] int userId)
        {
            try
            {
                var subs = await _serviceProvider.UserSubscriptionService.GetAllByUserIdAsync(userId);

                // Trả mảng rỗng thay vì 404 để FE xử lý dễ hơn
                return Ok(subs);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscriptions for user {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("payos-webhook")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Handle PayOS webhook", Description = "Handles payment webhook notifications from PayOS.")]
        [SwaggerResponse(200, "Webhook processed successfully.")]
        [SwaggerResponse(400, "Invalid webhook data.")]
        public async Task<IActionResult> HandleWebhook([FromBody] PayOSWebhookEnvelope body)
        {
            _logger.LogInformation("Webhook received: {@Body}", body);

            var d = body?.data;
            if (d == null)
            {
                // Ping/validate khi lưu Webhook URL có thể không đủ field -> luôn 200
                _logger.LogInformation("Webhook: missing data (likely ping) -> OK");
                return Ok();
            }

            // BỎ QUA SAMPLE/PING: PayOS hay gửi orderCode=123 khi bạn lưu Webhook URL
            if (d.orderCode == 123)
            {
                _logger.LogInformation("Webhook: ignore sample ping (orderCode=123) -> OK");
                return Ok();
            }

            // Xác định thành công
            bool isSuccess =
                string.Equals(body?.code, "00", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(d.code, "00", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(d.desc, "success", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(d.desc, "Thành công", StringComparison.OrdinalIgnoreCase);

            if (!isSuccess)
            {
                _logger.LogInformation("Webhook: not success (code/desc not OK) -> OK");
                return Ok();
            }

            long orderCode = d.orderCode;
            decimal amount = d.amount;

            try
            {
                // Tìm subscription theo orderCode (đã lưu khi create-payment)
                var subscription = await _serviceProvider.UserSubscriptionService
                    .GetByPaymentGatewayTransactionIdAsync(orderCode.ToString());

                if (subscription == null)
                {
                    // Không thấy -> có thể webhook đến trước khi bạn lưu pending, hoặc payload test
                    _logger.LogWarning("Webhook: subscription not found for order {OrderCode} -> OK", orderCode);
                    return Ok();
                }

                // Idempotent: đã COMPLETED thì bỏ qua
                if (string.Equals(subscription.PaymentStatus, "COMPLETED", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Webhook: order {OrderCode} already COMPLETED -> OK", orderCode);
                    return Ok();
                }

                // Lấy user và cập nhật balance
                var user = await _serviceProvider.UserService.GetUserByIdAsync(subscription.UserId);
                if (user == null)
                {
                    _logger.LogWarning("Webhook: user {UserId} not found for order {OrderCode} -> OK",
                        subscription.UserId, orderCode);
                    return Ok();
                }

                user.Balance ??= 0m;
                user.Balance += amount; // Trừ tiền khi thanh toán thành công
                await _serviceProvider.UserService.UpdateUserAsync(user);

                // Đánh dấu subscription COMPLETED
                subscription.PaymentStatus = "COMPLETED";
                await _serviceProvider.UserSubscriptionService.UpdateAsync(subscription);

                _logger.LogInformation(
                    "Webhook OK: order {OrderCode}; user {UserId} +{Amount}; new balance={Balance}",
                    orderCode, user.Id, amount, user.Balance
                );

                return Ok();
            }
            catch (Exception ex)
            {
                // DEV: vẫn trả 200 để PayOS không fail khi validate Webhook URL; log để sửa
                _logger.LogError(ex, "Webhook error for order {OrderCode} (dev -> return 200)", orderCode);
                return Ok();
            }
        }

    }

    public class CreatePaymentRequest
    {
        public int UserId { get; set; }
        public int? SubscriptionTypeId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class PayOSWebhookEnvelope
    {
        public string? code { get; set; }
        public string? desc { get; set; }
        public PayOSWebhookData? data { get; set; }
    }

    public class PayOSWebhookData
    {
        public long orderCode { get; set; }
        public long amount { get; set; }
        public string? description { get; set; }
        public string? code { get; set; }
        public string? desc { get; set; }
    }
}