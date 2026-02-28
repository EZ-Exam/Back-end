using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("user_overall_competencies")]
    public class UserOverallCompetency
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        [Required]
        [Column("SubjectId")]
        public int SubjectId { get; set; }

        [Column("OverallScore")]
        public decimal OverallScore { get; set; } = 0.0m;

        [Column("OverallAccuracy")]
        public decimal OverallAccuracy { get; set; } = 0.0m;

        [Column("TotalQuestionsAttempted")]
        public int TotalQuestionsAttempted { get; set; } = 0;

        [Column("TotalCorrectAnswers")]
        public int TotalCorrectAnswers { get; set; } = 0;

        [Column("TotalIncorrectAnswers")]
        public int TotalIncorrectAnswers { get; set; } = 0;

        [Column("TotalTimeSpent")]
        public int TotalTimeSpent { get; set; } = 0;

        [Column("AverageTimePerQuestion")]
        public decimal AverageTimePerQuestion { get; set; } = 0.0m;

        [Column("CompetencyLevel")]
        [MaxLength(20)]
        public string CompetencyLevel { get; set; } = "BEGINNER";

        [Column("ConfidenceScore")]
        public decimal ConfidenceScore { get; set; } = 0.0m;

        [Column("ConsistencyScore")]
        public decimal ConsistencyScore { get; set; } = 0.0m;

        [Column("ImprovementRate")]
        public decimal ImprovementRate { get; set; } = 0.0m;

        [Column("Strengths")]
        [MaxLength(2000)]
        public string? Strengths { get; set; }

        [Column("Weaknesses")]
        [MaxLength(2000)]
        public string? Weaknesses { get; set; }

        [Column("RecommendedFocusAreas")]
        [MaxLength(2000)]
        public string? RecommendedFocusAreas { get; set; }

        [Column("DifficultyBreakdown")]
        [MaxLength(1000)]
        public string? DifficultyBreakdown { get; set; }

        [Column("TopicBreakdown")]
        [MaxLength(2000)]
        public string? TopicBreakdown { get; set; }

        [Column("LearningVelocity")]
        public decimal LearningVelocity { get; set; } = 0.0m;

        [Column("RetentionRate")]
        public decimal RetentionRate { get; set; } = 0.0m;

        [Column("LastUpdated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; } = null!;
    }
}
