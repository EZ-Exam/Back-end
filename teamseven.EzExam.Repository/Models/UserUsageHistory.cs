using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("user_usage_history")]
    public class UserUsageHistory
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("UsageType")]
        public string UsageType { get; set; } = string.Empty;

        [Column("ResourceId")]
        public int? ResourceId { get; set; }

        [MaxLength(50)]
        [Column("ResourceType")]
        public string? ResourceType { get; set; }

        [MaxLength(500)]
        [Column("Description")]
        public string? Description { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}

