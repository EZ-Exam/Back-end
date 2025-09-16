namespace teamseven.EzExam.Services.Object.Responses
{
    public class SubscribeResponse
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public int SubscriptionTypeId { get; set; }
        public string SubscriptionCode { get; set; } = string.Empty;
        public string SubscriptionName { get; set; } = string.Empty;
        public decimal SubscriptionPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
