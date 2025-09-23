using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("test_session_answers")]
    public class TestSessionAnswer
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("TestSessionId")]
        public int TestSessionId { get; set; }

        [Required]
        [Column("QuestionId")]
        public int QuestionId { get; set; }

        [Column("SelectedAnswerId")]
        public int? SelectedAnswerId { get; set; }

        [Column("UserAnswer")]
        [MaxLength(5000)]
        public string? UserAnswer { get; set; } // For text-based answers

        [Column("IsCorrect")]
        public bool? IsCorrect { get; set; }

        [Column("TimeSpent")]
        public int TimeSpent { get; set; } = 0; // seconds spent on this question

        [Column("AnsweredAt")]
        public DateTime? AnsweredAt { get; set; }

        [Column("IsMarkedForReview")]
        public bool IsMarkedForReview { get; set; } = false;

        [Column("ConfidenceLevel")]
        public int? ConfidenceLevel { get; set; } // 1-5 scale

        [Column("AnswerSequence")]
        public int AnswerSequence { get; set; } = 0; // Order in which question was answered

        [Column("IsChanged")]
        public bool IsChanged { get; set; } = false; // If answer was changed

        [Column("ChangeCount")]
        public int ChangeCount { get; set; } = 0; // Number of times answer was changed

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("TestSessionId")]
        public virtual TestSession TestSession { get; set; } = null!;

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; } = null!;

        [ForeignKey("SelectedAnswerId")]
        public virtual Answer? SelectedAnswer { get; set; }
    }
}
