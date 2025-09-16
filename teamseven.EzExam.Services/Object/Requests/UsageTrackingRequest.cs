using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class UsageTrackingRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Usage type is required")]
        [StringLength(50, ErrorMessage = "Usage type cannot exceed 50 characters")]
        public string UsageType { get; set; } = string.Empty; // 'SOLUTION_VIEW', 'AI_REQUEST'

        [Range(1, int.MaxValue, ErrorMessage = "Resource ID must be a positive integer")]
        public int? ResourceId { get; set; }

        [StringLength(50, ErrorMessage = "Resource type cannot exceed 50 characters")]
        public string? ResourceType { get; set; } // 'SOLUTION', 'AI_CHAT'

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
    }
}
