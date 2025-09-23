using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("user_competency_assessments")]
    public class UserCompetencyAssessment
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

        [Column("ChapterId")]
        public int? ChapterId { get; set; }

        [Column("LessonId")]
        public int? LessonId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("DifficultyLevel")]
        public string DifficultyLevel { get; set; } = string.Empty; // EASY, MEDIUM, HARD

        [Required]
        [MaxLength(100)]
        [Column("Topic")]
        public string Topic { get; set; } = string.Empty; // e.g., "Kinematics", "Thermodynamics", "Calculus"

        [Column("TotalQuestions")]
        public int TotalQuestions { get; set; } = 0;

        [Column("CorrectAnswers")]
        public int CorrectAnswers { get; set; } = 0;

        [Column("IncorrectAnswers")]
        public int IncorrectAnswers { get; set; } = 0;

        [Column("AccuracyRate")]
        public decimal AccuracyRate { get; set; } = 0.0m; // percentage (0.0 to 100.0)

        [Column("CompetencyScore")]
        public decimal CompetencyScore { get; set; } = 0.0m; // weighted score (0.0 to 1.0)

        [Column("ConfidenceLevel")]
        public decimal ConfidenceLevel { get; set; } = 0.0m; // based on consistency (0.0 to 1.0)

        [Column("AverageTimePerQuestion")]
        public decimal AverageTimePerQuestion { get; set; } = 0.0m; // seconds

        [Column("TotalTimeSpent")]
        public int TotalTimeSpent { get; set; } = 0; // seconds

        [Column("LastAttemptedAt")]
        public DateTime? LastAttemptedAt { get; set; }

        [Column("FirstAttemptedAt")]
        public DateTime? FirstAttemptedAt { get; set; }

        [Column("ImprovementTrend")]
        public decimal ImprovementTrend { get; set; } = 0.0m; // positive = improving, negative = declining

        [Column("WeaknessAreas")]
        [MaxLength(1000)]
        public string? WeaknessAreas { get; set; } // JSON array of specific weak points

        [Column("StrengthAreas")]
        [MaxLength(1000)]
        public string? StrengthAreas { get; set; } // JSON array of strong points

        [Column("RecommendedActions")]
        [MaxLength(1000)]
        public string? RecommendedActions { get; set; } // JSON array of AI recommendations

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; } = null!;

        [ForeignKey("ChapterId")]
        public virtual Chapter? Chapter { get; set; }

        [ForeignKey("LessonId")]
        public virtual Lesson? Lesson { get; set; }
    }
}
