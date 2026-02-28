using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("ai_test_recommendations")]
    public class AITestRecommendation
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("TestName")]
        public string TestName { get; set; } = string.Empty;

        [MaxLength(1000)]
        [Column("Description")]
        public string? Description { get; set; }

        [Required]
        [Column("SubjectId")]
        public int SubjectId { get; set; }

        [Column("ChapterId")]
        public int? ChapterId { get; set; }

        [Column("LessonId")]
        public int? LessonId { get; set; }

        [Column("RecommendedDuration")]
        public int? RecommendedDuration { get; set; }

        [Column("RecommendedQuestionCount")]
        public int? RecommendedQuestionCount { get; set; }

        [Column("DifficultyDistribution")]
        [MaxLength(500)]
        public string? DifficultyDistribution { get; set; }

        [Column("TopicDistribution")]
        [MaxLength(1000)]
        public string? TopicDistribution { get; set; }

        [Column("BasedOnHistory")]
        public bool BasedOnHistory { get; set; } = true;

        [Column("BasedOnWeakAreas")]
        public bool BasedOnWeakAreas { get; set; } = true;

        [Column("BasedOnProgress")]
        public bool BasedOnProgress { get; set; } = true;

        [Column("ConfidenceScore")]
        public decimal ConfidenceScore { get; set; } = 0.0m;

        [Column("IsAccepted")]
        public bool? IsAccepted { get; set; }

        [Column("IsGenerated")]
        public bool IsGenerated { get; set; } = false;

        [Column("GeneratedExamId")]
        public int? GeneratedExamId { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("ExpiresAt")]
        public DateTime? ExpiresAt { get; set; }

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
