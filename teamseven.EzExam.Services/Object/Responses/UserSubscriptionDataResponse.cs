namespace teamseven.EzExam.Services.Object.Responses
{
    public class UserSubscriptionDataResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubscriptionTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public string PaymentStatus { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentGatewayTransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string ActionType { get; set; }
    }
}
