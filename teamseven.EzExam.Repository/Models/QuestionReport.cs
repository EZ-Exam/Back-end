using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("question_reports")]
    public class QuestionReport
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("QuestionId")]
        public int QuestionId { get; set; }

        [Required]
        [Column("ReportedByUserId")]
        public int ReportedByUserId { get; set; }

        [Required]
        [MaxLength(1000)]
        [Column("Reason")]
        public string Reason { get; set; } = string.Empty;

        [Column("ReportDate")]
        public DateTime ReportDate { get; set; } = DateTime.UtcNow;

        [Column("IsHandled")]
        public bool IsHandled { get; set; } = false;

        // Navigation properties
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; } = null!;

        [ForeignKey("ReportedByUserId")]
        public virtual User ReportedByUser { get; set; } = null!;
    }
}