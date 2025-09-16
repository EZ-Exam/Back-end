namespace teamseven.EzExam.Services.Object.Responses
{
    public class UsageTrackingResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubscriptionTypeId { get; set; }
        public string UsageType { get; set; } = string.Empty;
        public int UsageCount { get; set; }
        public DateTime ResetDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UsageHistoryResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UsageType { get; set; } = string.Empty;
        public int? ResourceId { get; set; }
        public string? ResourceType { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserSubscriptionStatusResponse
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string SubscriptionCode { get; set; } = string.Empty;
        public string SubscriptionName { get; set; } = string.Empty;
        public int MaxSolutionViews { get; set; }
        public int MaxAIRequests { get; set; }
        public bool IsAIEnabled { get; set; }
        public bool SubscriptionActive { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public int CurrentSolutionViews { get; set; }
        public int CurrentAIRequests { get; set; }
        public string RemainingSolutionViews { get; set; } = string.Empty;
        public string RemainingAIRequests { get; set; } = string.Empty;
        public bool CanViewSolution { get; set; }
        public bool CanUseAI { get; set; }
    }
}
