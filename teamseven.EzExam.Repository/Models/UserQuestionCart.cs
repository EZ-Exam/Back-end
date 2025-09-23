using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("user_question_carts")]
    public class UserQuestionCart
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        [Required]
        [Column("QuestionId")]
        public int QuestionId { get; set; }

        [Column("AddedAt")]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        [Column("IsSelected")]
        public bool IsSelected { get; set; } = true;

        [Column("UserNotes")]
        [MaxLength(1000)]
        public string? UserNotes { get; set; }

        [Column("DifficultyPreference")]
        [MaxLength(20)]
        public string? DifficultyPreference { get; set; } // EASY, MEDIUM, HARD

        [Column("SubjectId")]
        public int? SubjectId { get; set; }

        [Column("ChapterId")]
        public int? ChapterId { get; set; }

        [Column("LessonId")]
        public int? LessonId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; } = null!;

        [ForeignKey("SubjectId")]
        public virtual Subject? Subject { get; set; }

        [ForeignKey("ChapterId")]
        public virtual Chapter? Chapter { get; set; }

        [ForeignKey("LessonId")]
        public virtual Lesson? Lesson { get; set; }
    }
}
