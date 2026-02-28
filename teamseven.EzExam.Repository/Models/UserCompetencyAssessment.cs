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
        public string DifficultyLevel { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("Topic")]
        public string Topic { get; set; } = string.Empty;

        [Column("TotalQuestions")]
        public int TotalQuestions { get; set; } = 0;

        [Column("CorrectAnswers")]
        public int CorrectAnswers { get; set; } = 0;

        [Column("IncorrectAnswers")]
        public int IncorrectAnswers { get; set; } = 0;

        [Column("AccuracyRate")]
        public decimal AccuracyRate { get; set; } = 0.0m;

        [Column("CompetencyScore")]
        public decimal CompetencyScore { get; set; } = 0.0m;

        [Column("ConfidenceLevel")]
        public decimal ConfidenceLevel { get; set; } = 0.0m;

        [Column("AverageTimePerQuestion")]
        public decimal AverageTimePerQuestion { get; set; } = 0.0m;

        [Column("TotalTimeSpent")]
        public int TotalTimeSpent { get; set; } = 0;

        [Column("LastAttemptedAt")]
        public DateTime? LastAttemptedAt { get; set; }

        [Column("FirstAttemptedAt")]
        public DateTime? FirstAttemptedAt { get; set; }

        [Column("ImprovementTrend")]
        public decimal ImprovementTrend { get; set; } = 0.0m;

        [Column("WeaknessAreas")]
        [MaxLength(1000)]
        public string? WeaknessAreas { get; set; }

        [Column("StrengthAreas")]
        [MaxLength(1000)]
        public string? StrengthAreas { get; set; }

        [Column("RecommendedActions")]
        [MaxLength(1000)]
        public string? RecommendedActions { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

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
