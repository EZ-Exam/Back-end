using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("subscription_types")]
    public class SubscriptionType
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("SubscriptionCode")]
        public string SubscriptionCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("SubscriptionName")]
        public string SubscriptionName { get; set; } = string.Empty;

        [Column("SubscriptionPrice")]
        public decimal? SubscriptionPrice { get; set; }

        [MaxLength(500)]
        [Column("Description")]
        public string? Description { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedBy")]
        public int? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey("UpdatedBy")]
        public virtual User? UpdatedByNavigation { get; set; }

        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    }
}