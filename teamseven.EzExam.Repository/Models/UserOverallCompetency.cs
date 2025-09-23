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
        public decimal OverallScore { get; set; } = 0.0m; // 0.0 to 1.0

        [Column("OverallAccuracy")]
        public decimal OverallAccuracy { get; set; } = 0.0m; // percentage

        [Column("TotalQuestionsAttempted")]
        public int TotalQuestionsAttempted { get; set; } = 0;

        [Column("TotalCorrectAnswers")]
        public int TotalCorrectAnswers { get; set; } = 0;

        [Column("TotalIncorrectAnswers")]
        public int TotalIncorrectAnswers { get; set; } = 0;

        [Column("TotalTimeSpent")]
        public int TotalTimeSpent { get; set; } = 0; // seconds

        [Column("AverageTimePerQuestion")]
        public decimal AverageTimePerQuestion { get; set; } = 0.0m; // seconds

        [Column("CompetencyLevel")]
        [MaxLength(20)]
        public string CompetencyLevel { get; set; } = "BEGINNER"; // BEGINNER, INTERMEDIATE, ADVANCED, EXPERT

        [Column("ConfidenceScore")]
        public decimal ConfidenceScore { get; set; } = 0.0m; // 0.0 to 1.0

        [Column("ConsistencyScore")]
        public decimal ConsistencyScore { get; set; } = 0.0m; // 0.0 to 1.0

        [Column("ImprovementRate")]
        public decimal ImprovementRate { get; set; } = 0.0m; // percentage improvement over time

        [Column("Strengths")]
        [MaxLength(2000)]
        public string? Strengths { get; set; } // JSON array of strong areas

        [Column("Weaknesses")]
        [MaxLength(2000)]
        public string? Weaknesses { get; set; } // JSON array of weak areas

        [Column("RecommendedFocusAreas")]
        [MaxLength(2000)]
        public string? RecommendedFocusAreas { get; set; } // JSON array of areas to focus on

        [Column("DifficultyBreakdown")]
        [MaxLength(1000)]
        public string? DifficultyBreakdown { get; set; } // JSON: {"EASY": 0.9, "MEDIUM": 0.7, "HARD": 0.4}

        [Column("TopicBreakdown")]
        [MaxLength(2000)]
        public string? TopicBreakdown { get; set; } // JSON: {"Kinematics": 0.8, "Dynamics": 0.6, "Energy": 0.9}

        [Column("LearningVelocity")]
        public decimal LearningVelocity { get; set; } = 0.0m; // rate of improvement

        [Column("RetentionRate")]
        public decimal RetentionRate { get; set; } = 0.0m; // how well user retains knowledge

        [Column("LastUpdated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; } = null!;
    }
}
