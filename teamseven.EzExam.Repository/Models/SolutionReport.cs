using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("solution_reports")]
    public class SolutionReport
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("SolutionId")]
        public int SolutionId { get; set; }

        [Required]
        [Column("ReportedByUserId")]
        public int ReportedByUserId { get; set; }

        [Required]
        [MaxLength(1000)]
        [Column("Reason")]
        public string Reason { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("Status")]
        public string Status { get; set; } = "Pending";

        [Column("ReportDate")]
        public DateTime ReportDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("SolutionId")]
        public virtual Solution Solution { get; set; } = null!;

        [ForeignKey("ReportedByUserId")]
        public virtual User ReportedByUser { get; set; } = null!;
    }
}