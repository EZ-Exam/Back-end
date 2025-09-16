using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class SubscribeRequest
    {
        [Required(ErrorMessage = "Subscription type ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Subscription type ID must be greater than 0")]
        public int SubscriptionTypeId { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
    }
}

