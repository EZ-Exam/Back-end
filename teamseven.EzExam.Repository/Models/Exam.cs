using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("exams")]
    public class Exam
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        [Column("Description")]
        public string? Description { get; set; }

        [Required]
        [Column("SubjectId")]
        public int SubjectId { get; set; }

        [Column("LessonId")]
        public int? LessonId { get; set; }

        [Required]
        [Column("ExamTypeId")]
        public int ExamTypeId { get; set; }

        [Required]
        [Column("CreatedByUserId")]
        public int CreatedByUserId { get; set; }

        [Column("TimeLimit")]
        public int TimeLimit { get; set; } = 60;

        [Column("TotalQuestions")]
        public int TotalQuestions { get; set; } = 0;

        [Column("TotalMarks")]
        public int TotalMarks { get; set; } = 100;

        [Column("IsDeleted")]
        public bool IsDeleted { get; set; } = false;

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("IsPublic")]
        public bool IsPublic { get; set; } = false;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CreatedByUserId")]
        public virtual User CreatedByUser { get; set; } = null!;

        [ForeignKey("ExamTypeId")]
        public virtual ExamType ExamType { get; set; } = null!;

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; } = null!;

        [ForeignKey("LessonId")]
        public virtual Lesson? Lesson { get; set; }

        public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
        public virtual ICollection<ExamHistory> ExamHistories { get; set; } = new List<ExamHistory>();
    }
}