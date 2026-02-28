using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("exam_types")]
    public class ExamType
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("TypeCode")]
        public string TypeCode { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("Description")]
        public string? Description { get; set; }

        [Column("DefaultDuration")]
        public int? DefaultDuration { get; set; }

        [Column("MaxQuestions")]
        public int? MaxQuestions { get; set; }

        [Column("MinQuestions")]
        public int? MinQuestions { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
    }
}