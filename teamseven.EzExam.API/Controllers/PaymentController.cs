using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Services.ServiceProvider;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IServiceProviders _serviceProvider;  // Th�m Repository cho UserSubscription
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

            // T?o orderCode unique (s? d�ng l�m PaymentGatewayTransactionId)
            long orderCode = DateTimeOffset.Now.ToUnixTimeMilliseconds(); // 24/07/2025 03:18 PM +07

            // T?o ItemData cho PayOS
            var item = new ItemData(request.ItemName, request.Quantity, (int)request.Amount); // Chuy?n decimal sang int
            var items = new List<ItemData> { item };

            // C?u h�nh PaymentData
            var paymentData = new PaymentData(
                orderCode,
                (int)request.Amount, // Chuy?n decimal sang int v� PayOS y�u c?u
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
                SubscriptionTypeId = request.SubscriptionTypeId ?? 1, // Gi? s? m?c d?nh, b?n c� th? l?y t? request
                EndDate = DateTime.UtcNow.AddMonths(1), // Gi? s? g�i 1 th�ng t? 24/07/2025
                Amount = request.Amount,
                PaymentGatewayTransactionId = orderCode.ToString()
            };

            // Luu th�ng tin v�o UserSubscription qua service
            await _serviceProvider.UserSubscriptionService.AddSubscriptionAsync(subscriptionRequest);

            // Tr? v? URL cho FE
            return Ok(new { CheckoutUrl = result.checkoutUrl });
        }

        [HttpPost("payos-webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] WebhookType webhookBody)
        {
            Console.WriteLine("[Webhook] Received webhook from PayOS"); 
            try
            {
                WebhookData data = webhookBody.data;
                // N?u thanh to�n th�nh c�ng (code == "00")
                if (data.code == "00")
                {

                    var userSubscription = await _serviceProvider.UserSubscriptionService.GetByPaymentGatewayTransactionIdAsync(data.orderCode.ToString());
                    if (userSubscription != null)
                    {
                        Console.WriteLine("Found subscription: " + userSubscription.Id + data.amount+ userSubscription.UserId);

                        var user = await _serviceProvider.UserService.GetUserByIdAsync(userSubscription.UserId);
                        if (user != null)
                        {
                            Console.WriteLine("hi");

                            // X? l� Balance: N?u null th� g�n 0 tru?c khi c?ng amount
                            if (!user.Balance.HasValue)
                            {
                                user.Balance = 0m; // Chuy?n null th�nh 0
                            }
                            user.Balance += (decimal)data.amount; // Gi? s? Balance l� decimal
                            await _serviceProvider.UserService.UpdateUserAsync(user);
                        }
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    // DTO cho request
    public class CreatePaymentRequest
    {
        public int UserId { get; set; }
        public int? SubscriptionTypeId { get; set; }  // Th�m field n�y d? ch?n lo?i subscription
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }  // S? d?ng decimal d? kh?p v?i UserSubscription
        public string Description { get; set; }
    }
}
