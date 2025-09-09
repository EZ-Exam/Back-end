using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;

namespace teamseven.EzExam.Services.Services
{
    public class PayOSService : IPayOSService
    {
        private readonly PayOS _payOS;

        public PayOSService(IConfiguration configuration)
        {
            // Init PayOS v?i các key t? appsettings.json
            _payOS = new PayOS(
                configuration["PayOS:Environment:PAYOS_CLIENT_ID"] ?? throw new Exception("Missing PAYOS_CLIENT_ID"),
                configuration["PayOS:Environment:PAYOS_API_KEY"] ?? throw new Exception("Missing PAYOS_API_KEY"),
                configuration["PayOS:Environment:PAYOS_CHECKSUM_KEY"] ?? throw new Exception("Missing PAYOS_CHECKSUM_KEY"),
                configuration["PayOS:Environment:PAYOS_PARTNER_CODE"] ?? "" // optional
            );

        }

        public async Task<CreatePaymentResult> CreatePaymentLink(PaymentData paymentData)
        {
            return await _payOS.createPaymentLink(paymentData);
        }

        public WebhookData VerifyWebhook(WebhookType webhookBody)
        {
            return _payOS.verifyPaymentWebhookData(webhookBody);
        }

        public void ConfirmWebhook(string webhookUrl)
        {
            _payOS.confirmWebhook(webhookUrl);
        }
    }
}
