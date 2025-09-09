using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("user_subscriptions")]
    public class UserSubscription
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        [Required]
        [Column("SubscriptionTypeId")]
        public int SubscriptionTypeId { get; set; }

        [Column("Amount")]
        public decimal? Amount { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("PaymentStatus")]
        public string PaymentStatus { get; set; } = string.Empty;

        [MaxLength(255)]
        [Column("PaymentGatewayTransactionId")]
        public string? PaymentGatewayTransactionId { get; set; }

        [Column("StartDate")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Column("EndDate")]
        public DateTime? EndDate { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("SubscriptionTypeId")]
        public virtual SubscriptionType SubscriptionType { get; set; } = null!;
    }
}