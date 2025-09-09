using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Net.payOS.Types;
using teamseven.EzExam.Controllers;
using teamseven.EzExam.Repository.Dtos;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository.Repository;
using teamseven.EzExam.Repository.Repository.Interfaces;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Services;
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
        public async Task<IActionResult> HandleWebhook([FromBody] WebhookType webhookBody)
        {
            Console.WriteLine("[Webhook] Received webhook from PayOS"); 
            try
            {
                WebhookData data = webhookBody.data;
                // N?u thanh toán thành công (code == "00")
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

                            // X? lý Balance: N?u null thì gán 0 tru?c khi c?ng amount
                            if (!user.Balance.HasValue)
                            {
                                user.Balance = 0m; // Chuy?n null thành 0
                            }
                            user.Balance += (decimal)data.amount; // Gi? s? Balance là decimal
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
        public int? SubscriptionTypeId { get; set; }  // Thêm field này d? ch?n lo?i subscription
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }  // S? d?ng decimal d? kh?p v?i UserSubscription
        public string Description { get; set; }
    }
}
