using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("solutions")]
    public class Solution
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("QuestionId")]
        public int QuestionId { get; set; }

        [Required]
        [Column("CreatedByUserId")]
        public int CreatedByUserId { get; set; }

        [Required]
        [MaxLength(5000)]
        [Column("Content")]
        public string Content { get; set; } = string.Empty;

        [MaxLength(10000)]
        [Column("Explanation")]
        public string? Explanation { get; set; }

        [MaxLength(10000)]
        [Column("PythonScript")]
        public string? PythonScript { get; set; }

        [MaxLength(1000)]
        [Column("Mp4Url")]
        public string? Mp4Url { get; set; }

        [Column("IsApproved")]
        public bool IsApproved { get; set; } = true;

        [Column("IsDeleted")]
        public bool? IsDeleted { get; set; }

        [Column("IsMp4Generated")]
        public bool IsMp4Generated { get; set; } = false;

        [Column("IsMp4Reused")]
        public bool IsMp4Reused { get; set; } = false;

        [Column("OriginalSolutionId")]
        public int? OriginalSolutionId { get; set; }

        [Column("VideoData")]
        public byte[]? VideoData { get; set; }

        [MaxLength(255)]
        [Column("VideoContentType")]
        public string? VideoContentType { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; } = null!;

        [ForeignKey("CreatedByUserId")]
        public virtual User CreatedByUser { get; set; } = null!;

        [ForeignKey("OriginalSolutionId")]
        public virtual Solution? OriginalSolution { get; set; }

        public virtual ICollection<Solution> ReusedSolutions { get; set; } = new List<Solution>();
        public virtual ICollection<SolutionReport> SolutionReports { get; set; } = new List<SolutionReport>();
    }
}