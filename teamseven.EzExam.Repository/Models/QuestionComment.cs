using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("question_comments")]
    public class QuestionComment
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("QuestionId")]
        public int QuestionId { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(2000)]
        [Column("Content")]
        public string Content { get; set; } = string.Empty;

        [Column("ParentCommentId")]
        public int? ParentCommentId { get; set; }

        [Column("Rating")]
        public int Rating { get; set; } = 0;

        [Column("IsHelpful")]
        public bool IsHelpful { get; set; } = false;

        [Column("IsApproved")]
        public bool IsApproved { get; set; } = true;

        [Column("IsDeleted")]
        public bool IsDeleted { get; set; } = false;

        [Column("DeletedAt")]
        public DateTime? DeletedAt { get; set; }

        [Column("DeletedBy")]
        public int? DeletedBy { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("ParentCommentId")]
        public virtual QuestionComment? ParentComment { get; set; }

        public virtual ICollection<QuestionComment> Replies { get; set; } = new List<QuestionComment>();
    }
}
