using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("student_performance_summaries")]
    public class StudentPerformanceSummary
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        [Column("SubjectId")]
        public int? SubjectId { get; set; }

        [Column("GradeId")]
        public int? GradeId { get; set; }

        [Required]
        [Column("TotalQuizzesCompleted")]
        public int TotalQuizzesCompleted { get; set; } = 0;

        [Required]
        [Column("RecentQuizzesCount")]
        public int RecentQuizzesCount { get; set; } = 5; // Always track last 5 quizzes

        [Column("RecentQuizIds")]
        [MaxLength(200)]
        public string? RecentQuizIds { get; set; } // JSON array of last 5 quiz history IDs

        [Required]
        [Column("AverageScore")]
        public decimal AverageScore { get; set; } = 0.0m; // Average of last 5 quizzes

        [Required]
        [Column("AverageTimePerQuiz")]
        public decimal AverageTimePerQuiz { get; set; } = 0.0m; // seconds

        [Required]
        [Column("AverageTimePerQuestion")]
        public decimal AverageTimePerQuestion { get; set; } = 0.0m; // seconds

        [Column("OverallAccuracy")]
        public decimal OverallAccuracy { get; set; } = 0.0m; // percentage

        [Column("ImprovementTrend")]
        [MaxLength(20)]
        public string? ImprovementTrend { get; set; } // IMPROVING, STABLE, DECLINING

        [Column("TrendPercentage")]
        public decimal? TrendPercentage { get; set; } // percentage change over last 5 quizzes

        [Column("StrongTopics")]
        [MaxLength(1000)]
        public string? StrongTopics { get; set; } // JSON array of topics with >80% accuracy

        [Column("WeakTopics")]
        [MaxLength(1000)]
        public string? WeakTopics { get; set; } // JSON array of topics with <60% accuracy

        [Column("DifficultyProfile")]
        [MaxLength(500)]
        public string? DifficultyProfile { get; set; } // JSON: {"EASY": 85.5, "MEDIUM": 72.3, "HARD": 45.2}

        [Column("RecommendedDifficulty")]
        [MaxLength(20)]
        public string? RecommendedDifficulty { get; set; } // EASY, MEDIUM, HARD based on performance

        [Column("LearningVelocity")]
        public decimal LearningVelocity { get; set; } = 0.0m; // Rate of improvement (score increase per quiz)

        [Column("ConsistencyScore")]
        public decimal ConsistencyScore { get; set; } = 0.0m; // How consistent performance is (0-100)

        [Column("PredictedNextScore")]
        public decimal? PredictedNextScore { get; set; } // AI prediction for next quiz score

        [Column("ConfidenceLevel")]
        public decimal ConfidenceLevel { get; set; } = 0.0m; // Student's average confidence level

        [Column("TimeManagementScore")]
        public decimal TimeManagementScore { get; set; } = 0.0m; // How well student manages time (0-100)

        [Column("LastQuizDate")]
        public DateTime? LastQuizDate { get; set; }

        [Column("LastAnalysisDate")]
        public DateTime LastAnalysisDate { get; set; } = DateTime.UtcNow;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("SubjectId")]
        public virtual Subject? Subject { get; set; }

        [ForeignKey("GradeId")]
        public virtual Grade? Grade { get; set; }
    }
}
