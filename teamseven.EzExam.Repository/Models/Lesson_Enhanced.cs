using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("lessons")]
    public class Lesson_Enhanced
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("Title")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        [Column("Description")]
        public string? Description { get; set; }

        [Required]
        [Column("ChapterId")]
        public int ChapterId { get; set; }

        [MaxLength(500)]
        [Column("PdfUrl")]
        public string? PdfUrl { get; set; }

        [MaxLength(500)]
        [Column("VideoUrl")]
        public string? VideoUrl { get; set; }

        [Column("Order")]
        public int Order { get; set; } = 1;

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ChapterId")]
        public virtual Chapter Chapter { get; set; } = null!;

        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
        public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
    }
}
