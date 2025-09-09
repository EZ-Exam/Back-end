using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("exam_histories")]
    public class ExamHistory
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("ExamId")]
        public int ExamId { get; set; }

        [Required]
        [Column("ActionByUserId")]
        public int ActionByUserId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("Action")]
        public string Action { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("Description")]
        public string? Description { get; set; }

        [Column("ActionDate")]
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; } = null!;

        [ForeignKey("ActionByUserId")]
        public virtual User ActionByUser { get; set; } = null!;
    }
}