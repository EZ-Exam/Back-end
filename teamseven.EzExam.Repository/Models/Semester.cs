using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("semesters")]
    public class Semester
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("GradeId")]
        public int GradeId { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("GradeId")]
        public virtual Grade Grade { get; set; } = null!;

        public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    }
}