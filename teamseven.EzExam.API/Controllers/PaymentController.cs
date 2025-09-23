using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Net.payOS.Types;
using Swashbuckle.AspNetCore.Annotations;
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
                    "https://fe-phy-gen.vercel.app/",
                    "https://fe-phy-gen.vercel.app/"
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

        [HttpPost("payos-webhook")]
        [SwaggerOperation(Summary = "Handle PayOS webhook", Description = "Handles payment webhook notifications from PayOS.")]
        [SwaggerResponse(200, "Webhook processed successfully.")]
        [SwaggerResponse(400, "Invalid webhook data.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> HandleWebhook([FromBody] PayOSWebhookEnvelope body)
        {
            _logger.LogInformation("Webhook received: {@Body}", body);

            var webhookData = body?.data;
            if (webhookData == null)
            {
                _logger.LogWarning("Webhook received with no data");
                return Ok();
            }

            bool isSuccess =
                string.Equals(body?.code, "00", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(webhookData.code, "00", StringComparison.OrdinalIgnoreCase);

            if (!isSuccess)
            {
                _logger.LogWarning("Webhook received with unsuccessful payment status");
                return Ok();
            }

            long orderCode = webhookData.orderCode;
            decimal amount = webhookData.amount;

            try
            {
                // Find UserSubscription by orderCode
                var subscription = await _serviceProvider.UserSubscriptionService
                    .GetByPaymentGatewayTransactionIdAsync(orderCode.ToString());

                if (subscription == null)
                {
                    _logger.LogWarning("Webhook: subscription not found for order {OrderCode}", orderCode);
                    return Ok();
                }

                // Get user by subscription
                var user = await _serviceProvider.UserService.GetUserByIdAsync(subscription.UserId);
                if (user == null)
                {
                    _logger.LogWarning("Webhook: user {UserId} not found for order {OrderCode}", subscription.UserId, orderCode);
                    return Ok();
                }

                // Update balance
                user.Balance ??= 0m;
                user.Balance += amount;
                await _serviceProvider.UserService.UpdateUserAsync(user);

                // Set subscription status to COMPLETED
                subscription.PaymentStatus = "COMPLETED";
                await _serviceProvider.UserSubscriptionService.UpdateAsync(subscription);

                _logger.LogInformation(
                    "Webhook: Order {OrderCode} completed. User {UserId} balance +{Amount}, new balance={Balance}",
                    orderCode, user.Id, amount, user.Balance
                );

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook for order {OrderCode}", orderCode);
                return StatusCode(500, new { Message = "An error occurred while processing the webhook." });
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