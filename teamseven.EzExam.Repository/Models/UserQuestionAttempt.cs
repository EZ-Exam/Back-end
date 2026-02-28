using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("user_question_attempts")]
    public class UserQuestionAttempt
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        [Required]
        [Column("QuestionId")]
        public int QuestionId { get; set; }

        [Column("SelectedAnswerId")]
        public int? SelectedAnswerId { get; set; }

        [Column("UserAnswer")]
        [MaxLength(5000)]
        public string? UserAnswer { get; set; }

        [Column("IsCorrect")]
        public bool IsCorrect { get; set; }

        [Column("TimeSpent")]
        public int TimeSpent { get; set; } = 0;

        [Column("ConfidenceLevel")]
        public int? ConfidenceLevel { get; set; }

        [Column("AttemptType")]
        [MaxLength(50)]
        public string AttemptType { get; set; } = "PRACTICE";

        [Column("SessionId")]
        public int? SessionId { get; set; }

        [Column("ExamId")]
        public int? ExamId { get; set; }

        [Column("DifficultyLevel")]
        [MaxLength(20)]
        public string? DifficultyLevel { get; set; }

        [Column("Topic")]
        [MaxLength(100)]
        public string? Topic { get; set; }

        [Column("SubjectId")]
        public int? SubjectId { get; set; }

        [Column("ChapterId")]
        public int? ChapterId { get; set; }

        [Column("LessonId")]
        public int? LessonId { get; set; }

        [Column("IsHintUsed")]
        public bool IsHintUsed { get; set; } = false;

        [Column("HintCount")]
        public int HintCount { get; set; } = 0;

        [Column("IsSkipped")]
        public bool IsSkipped { get; set; } = false;

        [Column("IsMarkedForReview")]
        public bool IsMarkedForReview { get; set; } = false;

        [Column("AttemptSequence")]
        public int AttemptSequence { get; set; } = 1;

        [Column("PreviousAttempts")]
        public int PreviousAttempts { get; set; } = 0;

        [Column("LearningOutcome")]
        [MaxLength(500)]
        public string? LearningOutcome { get; set; }

        [Column("MistakeType")]
        [MaxLength(100)]
        public string? MistakeType { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; } = null!;

        [ForeignKey("SelectedAnswerId")]
        public virtual Answer? SelectedAnswer { get; set; }

        [ForeignKey("SessionId")]
        public virtual TestSession? TestSession { get; set; }

        [ForeignKey("ExamId")]
        public virtual Exam? Exam { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subject? Subject { get; set; }

        [ForeignKey("ChapterId")]
        public virtual Chapter? Chapter { get; set; }

        [ForeignKey("LessonId")]
        public virtual Lesson? Lesson { get; set; }
    }
}
