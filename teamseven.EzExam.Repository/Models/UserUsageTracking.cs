using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("user_usage_tracking")]
    public class UserUsageTracking
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

        [Required]
        [MaxLength(50)]
        [Column("UsageType")]
        public string UsageType { get; set; } = string.Empty; // 'SOLUTION_VIEW', 'AI_REQUEST'

        [Column("UsageCount")]
        public int UsageCount { get; set; } = 0;

        [Required]
        [Column("ResetDate")]
        public DateTime ResetDate { get; set; }

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

