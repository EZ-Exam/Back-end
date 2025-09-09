using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("answers")]
    public class Answer
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("QuestionId")]
        public int QuestionId { get; set; }

        [Required]
        [MaxLength(10)]
        [Column("AnswerKey")]
        public string AnswerKey { get; set; } = string.Empty;

        [Required]
        [MaxLength(5000)]
        [Column("Content")]
        public string Content { get; set; } = string.Empty;

        [Column("IsCorrect")]
        public bool IsCorrect { get; set; } = false;

        [MaxLength(5000)]
        [Column("Explanation")]
        public string? Explanation { get; set; }

        [Column("Order")]
        public int Order { get; set; } = 1;

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; } = null!;
    }
}
