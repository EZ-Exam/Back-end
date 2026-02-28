using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("user_learning_histories")]
    public class UserLearningHistory
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

        [Column("QuestionId")]
        public int? QuestionId { get; set; }

        [Column("ExamId")]
        public int? ExamId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("ActivityType")]
        public string ActivityType { get; set; } = string.Empty;

        [Column("TimeSpent")]
        public int TimeSpent { get; set; } = 0;

        [Column("Score")]
        public decimal? Score { get; set; }

        [Column("IsCorrect")]
        public bool? IsCorrect { get; set; }

        [Column("DifficultyLevel")]
        [MaxLength(20)]
        public string? DifficultyLevel { get; set; }

        [Column("TopicTags")]
        [MaxLength(500)]
        public string? TopicTags { get; set; }

        [Column("WeakAreas")]
        [MaxLength(1000)]
        public string? WeakAreas { get; set; }

        [Column("Strengths")]
        [MaxLength(1000)]
        public string? Strengths { get; set; }

        [Column("LearningProgress")]
        public decimal LearningProgress { get; set; } = 0.0m;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; } = null!;

        [ForeignKey("ChapterId")]
        public virtual Chapter? Chapter { get; set; }

        [ForeignKey("LessonId")]
        public virtual Lesson? Lesson { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question? Question { get; set; }

        [ForeignKey("ExamId")]
        public virtual Exam? Exam { get; set; }
    }
}
