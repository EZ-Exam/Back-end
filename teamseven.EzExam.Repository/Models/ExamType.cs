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
        public string TypeCode { get; set; } = string.Empty; // MOCK, AI_GEN, USER

        [MaxLength(500)]
        [Column("Description")]
        public string? Description { get; set; }

        [Column("DefaultDuration")]
        public int? DefaultDuration { get; set; } // minutes - optional default, user can override

        [Column("MaxQuestions")]
        public int? MaxQuestions { get; set; } // optional limit

        [Column("MinQuestions")]
        public int? MinQuestions { get; set; } // optional minimum

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
    }
}