using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class SubscriptionTypeRequest
    {
        [Required(ErrorMessage = "Subscription code is required")]
        [StringLength(50, ErrorMessage = "Subscription code cannot exceed 50 characters")]
        public string SubscriptionCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subscription name is required")]
        [StringLength(255, ErrorMessage = "Subscription name cannot exceed 255 characters")]
        public string SubscriptionName { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Subscription price must be non-negative")]
        public decimal? SubscriptionPrice { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Max solution views must be non-negative or -1 for unlimited")]
        public int MaxSolutionViews { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "Max AI requests must be non-negative or -1 for unlimited")]
        public int MaxAIRequests { get; set; } = 0;

        public bool IsAIEnabled { get; set; } = false;

        [StringLength(2000, ErrorMessage = "Features cannot exceed 2000 characters")]
        public string? Features { get; set; }

        public bool IsActive { get; set; } = true;

        [Range(1, int.MaxValue, ErrorMessage = "Updated by user ID is required")]
        public int UpdatedBy { get; set; }
    }
}