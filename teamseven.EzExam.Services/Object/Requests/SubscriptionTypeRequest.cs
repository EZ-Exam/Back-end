using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Repository.Dtos
{
    public class SubscriptionTypeRequest
    {
        [Required]
        [StringLength(50)]
        public string SubscriptionCode { get; set; }

        [Required]
        [StringLength(255)]
        public string SubscriptionName { get; set; }

        [Range(0, double.MaxValue)]
        public decimal SubscriptionPrice { get; set; }

        public int? DurationInDays { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public long UpdatedBy { get; set; }
    }
}
