using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("lessons_enhanced")]
    public class LessonEnhanced
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        [Column("Title")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        [Column("Description")]
        public string? Description { get; set; }

        [Column("SubjectId")]
        public int SubjectId { get; set; }

        [MaxLength(500)]
        [Column("PdfUrl")]
        public string? PdfUrl { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Nav to join rows
        public ICollection<LessonEnhancedQuestion> LessonQuestions { get; set; } = new List<LessonEnhancedQuestion>();
    }

    [Table("lessons_enhanced_questions")]
    public class LessonEnhancedQuestion
    {
        [Column("LessonId")]
        public int LessonId { get; set; }

        [Column("QuestionId")]
        public int QuestionId { get; set; }

        [Column("Position")]
        public int Position { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navs
        public LessonEnhanced Lesson { get; set; } = null!;
        // (Không cần nav Question entity nếu bạn chỉ lưu id; thêm cũng được nếu bạn có model Question)
        public Question? Question { get; set; } = null!;
    }
}
