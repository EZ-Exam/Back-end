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
        public string? UserAnswer { get; set; } // For text-based answers

        [Column("IsCorrect")]
        public bool IsCorrect { get; set; }

        [Column("TimeSpent")]
        public int TimeSpent { get; set; } = 0; // seconds

        [Column("ConfidenceLevel")]
        public int? ConfidenceLevel { get; set; } // 1-5 scale (1 = very unsure, 5 = very confident)

        [Column("AttemptType")]
        [MaxLength(50)]
        public string AttemptType { get; set; } = "PRACTICE"; // PRACTICE, EXAM, REVIEW, QUIZ

        [Column("SessionId")]
        public int? SessionId { get; set; } // Link to TestSession if applicable

        [Column("ExamId")]
        public int? ExamId { get; set; } // Link to Exam if applicable

        [Column("DifficultyLevel")]
        [MaxLength(20)]
        public string? DifficultyLevel { get; set; } // EASY, MEDIUM, HARD

        [Column("Topic")]
        [MaxLength(100)]
        public string? Topic { get; set; } // Topic of the question

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
        public int AttemptSequence { get; set; } = 1; // 1st attempt, 2nd attempt, etc.

        [Column("PreviousAttempts")]
        public int PreviousAttempts { get; set; } = 0; // Number of previous attempts on this question

        [Column("LearningOutcome")]
        [MaxLength(500)]
        public string? LearningOutcome { get; set; } // What the user learned from this attempt

        [Column("MistakeType")]
        [MaxLength(100)]
        public string? MistakeType { get; set; } // e.g., "Calculation Error", "Concept Misunderstanding"

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
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
