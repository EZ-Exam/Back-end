using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using teamseven.EzExam.Controllers;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Services.ServiceProvider;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;  // Thêm Repository cho UserSubscription
        private readonly ILogger<ChapterController> _logger;

        public PaymentController(ILogger<ChapterController> logger, IServiceProviders serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment(CreatePaymentRequest request)
        {
            if (request == null || request.UserId <= 0 || request.Amount <= 0)
            {
                return BadRequest(new { Message = "Invalid request data" });
            }

            // T?o orderCode unique (s? dùng làm PaymentGatewayTransactionId)
            long orderCode = DateTimeOffset.Now.ToUnixTimeMilliseconds(); // 24/07/2025 03:18 PM +07

            // T?o ItemData cho PayOS
            var item = new ItemData(request.ItemName, request.Quantity, (int)request.Amount); // Chuy?n decimal sang int
            var items = new List<ItemData> { item };

            // C?u hình PaymentData
            var paymentData = new PaymentData(
                orderCode,
                (int)request.Amount, // Chuy?n decimal sang int vì PayOS yêu c?u
                request.Description,
                items,
                "https://fe-phy-gen.vercel.app/",
                "https://fe-phy-gen.vercel.app/"
            );
            // G?i service d? t?o payment link
            CreatePaymentResult result = await _serviceProvider.PayOSService.CreatePaymentLink(paymentData);

            // Chu?n b? UserSubscriptionRequest d? g?i AddSubscriptionAsync
            var subscriptionRequest = new UserSubscriptionRequest
            {
                UserId = request.UserId,
                SubscriptionTypeId = request.SubscriptionTypeId ?? 1, // Gi? s? m?c d?nh, b?n có th? l?y t? request
                EndDate = DateTime.UtcNow.AddMonths(1), // Gi? s? gói 1 tháng t? 24/07/2025
                Amount = request.Amount,
                PaymentGatewayTransactionId = orderCode.ToString()
            };

            // Luu thông tin vào UserSubscription qua service
            await _serviceProvider.UserSubscriptionService.AddSubscriptionAsync(subscriptionRequest);

            // Tr? v? URL cho FE
            return Ok(new { CheckoutUrl = result.checkoutUrl });
        }

        [HttpPost("payos-webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] PayOSWebhookEnvelope body)
        {
            _logger.LogInformation("Webhook received: {@Body}", body);

            var d = body?.data;
            if (d == null) return Ok(); // không có data → bỏ qua

            bool isSuccess =
                string.Equals(body?.code, "00", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(d.code, "00", StringComparison.OrdinalIgnoreCase);

            if (!isSuccess) return Ok();

            long orderCode = d.orderCode;
            decimal amount = d.amount;

            // 🔍 Tìm UserSubscription theo orderCode
            var subscription = await _serviceProvider.UserSubscriptionService
                .GetByPaymentGatewayTransactionIdAsync(orderCode.ToString());

            if (subscription == null)
            {
                _logger.LogWarning("Webhook: subscription not found for order {OrderCode}", orderCode);
                return Ok();
            }

            // 🔍 Lấy user theo subscription
            var user = await _serviceProvider.UserService.GetUserByIdAsync(subscription.UserId);
            if (user == null)
            {
                _logger.LogWarning("Webhook: user {UserId} not found for order {OrderCode}", subscription.UserId, orderCode);
                return Ok();
            }

            // 💰 Update balance
            user.Balance ??= 0m;
            user.Balance += amount;
            await _serviceProvider.UserService.UpdateUserAsync(user);

            // ✅ Set subscription status = COMPLETED
            subscription.PaymentStatus = "COMPLETED";
            await _serviceProvider.UserSubscriptionService.UpdateAsync(subscription);

            _logger.LogInformation(
                "Webhook: Order {OrderCode} completed. User {UserId} balance +{Amount}, new balance={Balance}",
                orderCode, user.Id, amount, user.Balance
            );

            return Ok();
        }

    }

    // DTO cho request
    public class CreatePaymentRequest
    {
        public int UserId { get; set; }
        public int? SubscriptionTypeId { get; set; }  // Thêm field này d? ch?n lo?i subscription
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }  // S? d?ng decimal d? kh?p v?i UserSubscription
        public string Description { get; set; }
    }
    public class PayOSWebhookEnvelope
    {
        public string? code { get; set; }     // "00"
        public string? desc { get; set; }     // "success"
        public PayOSWebhookData? data { get; set; }
    }

    public class PayOSWebhookData
    {
        public long orderCode { get; set; }
        public long amount { get; set; }
        public string? description { get; set; }
        public string? code { get; set; }     // "00"
        public string? desc { get; set; }     // "Thành công"
    }
}
