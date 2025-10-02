using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("student_question_attempts")]
    public class StudentQuestionAttempt
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("StudentQuizHistoryId")]
        public int StudentQuizHistoryId { get; set; }

        [Required]
        [Column("QuestionId")]
        public int QuestionId { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        [Column("SelectedAnswerId")]
        public int? SelectedAnswerId { get; set; }

        [Column("UserAnswer")]
        [MaxLength(5000)]
        public string? UserAnswer { get; set; } // For text-based answers

        [Required]
        [Column("IsCorrect")]
        public bool IsCorrect { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("DifficultyLevel")]
        public string DifficultyLevel { get; set; } = "MEDIUM"; // EASY, MEDIUM, HARD

        [Column("TimeSpent")]
        public int TimeSpent { get; set; } = 0; // seconds spent on this question

        [Column("Topic")]
        [MaxLength(200)]
        public string? Topic { get; set; } // Topic/subject of the question

        [Column("ChapterId")]
        public int? ChapterId { get; set; }

        [Column("LessonId")]
        public int? LessonId { get; set; }

        [Column("QuestionOrder")]
        public int QuestionOrder { get; set; } = 0; // Order of question in the quiz

        [Column("ConfidenceLevel")]
        public int? ConfidenceLevel { get; set; } // 1-5 scale (1 = very unsure, 5 = very confident)

        [Column("IsMarkedForReview")]
        public bool IsMarkedForReview { get; set; } = false;

        [Column("IsSkipped")]
        public bool IsSkipped { get; set; } = false;

        [Column("AnswerChangeCount")]
        public int AnswerChangeCount { get; set; } = 0; // Number of times answer was changed

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("StudentQuizHistoryId")]
        public virtual StudentQuizHistory StudentQuizHistory { get; set; } = null!;

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("SelectedAnswerId")]
        public virtual Answer? SelectedAnswer { get; set; }

        [ForeignKey("ChapterId")]
        public virtual Chapter? Chapter { get; set; }

        [ForeignKey("LessonId")]
        public virtual Lesson? Lesson { get; set; }
    }
}
