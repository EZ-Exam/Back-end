using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("chatbots")]
    public class Chatbot
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(2000)]
        [Column("UserMessage")]
        public string UserMessage { get; set; } = string.Empty;

        [Required]
        [MaxLength(5000)]
        [Column("BotResponse")]
        public string BotResponse { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("MessageType")]
        public string MessageType { get; set; } = "TEXT";

        [MaxLength(100)]
        [Column("Context")]
        public string? Context { get; set; }

        [Column("QuestionId")]
        public int? QuestionId { get; set; }

        [Column("SubjectId")]
        public int? SubjectId { get; set; }

        [Column("IsHelpful")]
        public bool IsHelpful { get; set; } = false;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("QuestionId")]
        public virtual Question? Question { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subject? Subject { get; set; }
    }
}
