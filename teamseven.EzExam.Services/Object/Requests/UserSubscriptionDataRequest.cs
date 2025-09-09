namespace teamseven.EzExam.Services.Object.Requests
{
    public class UserSubscriptionDataRequest
    {
        public int Id { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public string PaymentStatus { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentGatewayTransactionId { get; set; }
    }
}
