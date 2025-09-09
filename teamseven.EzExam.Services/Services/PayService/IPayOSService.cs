using Net.payOS.Types;

namespace teamseven.EzExam.Services.Services
{
    public interface IPayOSService
    {
        Task<CreatePaymentResult> CreatePaymentLink(PaymentData paymentData);
        WebhookData VerifyWebhook(WebhookType webhookBody);
        void ConfirmWebhook(string webhookUrl);
    }
}
