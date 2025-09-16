namespace teamseven.EzExam.Services.Object.Responses
{
    public class SubscriptionTypeResponse
    {
        public int Id { get; set; }
        public string SubscriptionCode { get; set; } = string.Empty;
        public string SubscriptionName { get; set; } = string.Empty;
        public decimal? SubscriptionPrice { get; set; }
        public string? Description { get; set; }
        public int MaxSolutionViews { get; set; }
        public int MaxAIRequests { get; set; }
        public bool IsAIEnabled { get; set; }
        public string? Features { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
    }
}