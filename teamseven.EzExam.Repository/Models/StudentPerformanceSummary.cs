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
        public int RecentQuizzesCount { get; set; } = 5;

        [Column("RecentQuizIds")]
        [MaxLength(200)]
        public string? RecentQuizIds { get; set; }

        [Required]
        [Column("AverageScore")]
        public decimal AverageScore { get; set; } = 0.0m;

        [Required]
        [Column("AverageTimePerQuiz")]
        public decimal AverageTimePerQuiz { get; set; } = 0.0m;

        [Required]
        [Column("AverageTimePerQuestion")]
        public decimal AverageTimePerQuestion { get; set; } = 0.0m;

        [Column("OverallAccuracy")]
        public decimal OverallAccuracy { get; set; } = 0.0m;

        [Column("ImprovementTrend")]
        [MaxLength(20)]
        public string? ImprovementTrend { get; set; }

        [Column("TrendPercentage")]
        public decimal? TrendPercentage { get; set; }

        [Column("StrongTopics")]
        [MaxLength(1000)]
        public string? StrongTopics { get; set; }

        [Column("WeakTopics")]
        [MaxLength(1000)]
        public string? WeakTopics { get; set; }

        [Column("DifficultyProfile")]
        [MaxLength(500)]
        public string? DifficultyProfile { get; set; }

        [Column("RecommendedDifficulty")]
        [MaxLength(20)]
        public string? RecommendedDifficulty { get; set; }

        [Column("LearningVelocity")]
        public decimal LearningVelocity { get; set; } = 0.0m;

        [Column("ConsistencyScore")]
        public decimal ConsistencyScore { get; set; } = 0.0m;

        [Column("PredictedNextScore")]
        public decimal? PredictedNextScore { get; set; }

        [Column("ConfidenceLevel")]
        public decimal ConfidenceLevel { get; set; } = 0.0m;

        [Column("TimeManagementScore")]
        public decimal TimeManagementScore { get; set; } = 0.0m;

        [Column("LastQuizDate")]
        public DateTime? LastQuizDate { get; set; }

        [Column("LastAnalysisDate")]
        public DateTime LastAnalysisDate { get; set; } = DateTime.UtcNow;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("SubjectId")]
        public virtual Subject? Subject { get; set; }

        [ForeignKey("GradeId")]
        public virtual Grade? Grade { get; set; }
    }
}
