using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("lessons")]
    public class Lesson
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("ChapterId")]
        public int ChapterId { get; set; }

        [Column("GradeId")]
        public int? GradeId { get; set; }

        [MaxLength(5000)]
        [Column("Document")]
        public string? Document { get; set; }

        [MaxLength(50)]
        [Column("DocumentType")]
        public string? DocumentType { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ChapterId")]
        public virtual Chapter Chapter { get; set; } = null!;

        [ForeignKey("GradeId")]
        public virtual Grade? Grade { get; set; }

        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
        public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
    }
}