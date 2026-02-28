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
        public string? UserAnswer { get; set; }

        [Column("IsCorrect")]
        public bool? IsCorrect { get; set; }

        [Column("TimeSpent")]
        public int TimeSpent { get; set; } = 0;

        [Column("AnsweredAt")]
        public DateTime? AnsweredAt { get; set; }

        [Column("IsMarkedForReview")]
        public bool IsMarkedForReview { get; set; } = false;

        [Column("ConfidenceLevel")]
        public int? ConfidenceLevel { get; set; }

        [Column("AnswerSequence")]
        public int AnswerSequence { get; set; } = 0;

        [Column("IsChanged")]
        public bool IsChanged { get; set; } = false;

        [Column("ChangeCount")]
        public int ChangeCount { get; set; } = 0;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("TestSessionId")]
        public virtual TestSession TestSession { get; set; } = null!;

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; } = null!;

        [ForeignKey("SelectedAnswerId")]
        public virtual Answer? SelectedAnswer { get; set; }
    }
}
