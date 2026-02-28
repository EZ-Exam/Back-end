using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("student_quiz_histories")]
    public class StudentQuizHistory
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        [Required]
        [Column("ExamId")]
        public int ExamId { get; set; }

        [Column("TestSessionId")]
        public int? TestSessionId { get; set; }

        [Required]
        [Column("StartedAt")]
        public DateTime StartedAt { get; set; }

        [Column("CompletedAt")]
        public DateTime? CompletedAt { get; set; }

        [Required]
        [Column("TimeSpent")]
        public int TimeSpent { get; set; } = 0;

        [Required]
        [Column("TotalQuestions")]
        public int TotalQuestions { get; set; }

        [Required]
        [Column("CorrectAnswers")]
        public int CorrectAnswers { get; set; } = 0;

        [Required]
        [Column("IncorrectAnswers")]
        public int IncorrectAnswers { get; set; } = 0;

        [Column("SkippedQuestions")]
        public int SkippedQuestions { get; set; } = 0;

        [Required]
        [Column("TotalScore")]
        public decimal TotalScore { get; set; } = 0.0m;

        [Column("PassingScore")]
        public decimal? PassingScore { get; set; }

        [Column("IsPassed")]
        public bool? IsPassed { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("QuizStatus")]
        public string QuizStatus { get; set; } = "COMPLETED";

        [Column("AverageTimePerQuestion")]
        public decimal AverageTimePerQuestion { get; set; } = 0.0m;

        [Column("DifficultyBreakdown")]
        [MaxLength(1000)]
        public string? DifficultyBreakdown { get; set; }

        [Column("TopicPerformance")]
        [MaxLength(2000)]
        public string? TopicPerformance { get; set; }

        [Column("WeakAreas")]
        [MaxLength(1000)]
        public string? WeakAreas { get; set; }

        [Column("StrongAreas")]
        [MaxLength(1000)]
        public string? StrongAreas { get; set; }

        [Column("ImprovementAreas")]
        [MaxLength(1000)]
        public string? ImprovementAreas { get; set; }

        [Column("PerformanceRating")]
        [MaxLength(20)]
        public string? PerformanceRating { get; set; }

        [Column("ComparedToPrevious")]
        public decimal? ComparedToPrevious { get; set; }

        [Column("DeviceInfo")]
        [MaxLength(500)]
        public string? DeviceInfo { get; set; }

        [Column("SessionData")]
        [MaxLength(10000)]
        public string? SessionData { get; set; }

        [Column("IsCheatingDetected")]
        public bool IsCheatingDetected { get; set; } = false;

        [Column("CheatingDetails")]
        [MaxLength(1000)]
        public string? CheatingDetails { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; } = null!;

        [ForeignKey("TestSessionId")]
        public virtual TestSession? TestSession { get; set; }
    }
}
