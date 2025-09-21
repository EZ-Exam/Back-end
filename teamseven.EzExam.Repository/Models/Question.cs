using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("questions")]
    public class Question
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(5000)]
        [Column("Content")]
        public string Content { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("QuestionSource")]
        public string? QuestionSource { get; set; }

        [Required]
        [Column("DifficultyLevelId")]
        public int DifficultyLevelId { get; set; }

        [Required]
        [Column("SubjectId")]
        public int SubjectId { get; set; }

        [Column("ChapterId")]
        public int? ChapterId { get; set; }

        [Column("LessonId")]
        public int? LessonId { get; set; }

        [Column("TextbookId")]
        public int? TextbookId { get; set; }

        [Required]
        [Column("CreatedByUserId")]
        public int CreatedByUserId { get; set; }

        [MaxLength(20)]
        [Column("QuestionType")]
        public string QuestionType { get; set; } = "MULTIPLE_CHOICE";

        [MaxLength(5000)]
        [Column("Image")]
        public string? Image { get; set; }


        [Column("TemplateQuestionId")]
        public int? TemplateQuestionId { get; set; }

        [Column("IsCloned")]
        public bool IsCloned { get; set; } = false;

        [Column("ViewCount")]
        public int ViewCount { get; set; } = 0;

        [Column("AverageRating")]
        public decimal AverageRating { get; set; } = 0;

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CreatedByUserId")]
        public virtual User CreatedByUser { get; set; } = null!;

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; } = null!;

        [ForeignKey("DifficultyLevelId")]
        public virtual DifficultyLevel DifficultyLevel { get; set; } = null!;

        [ForeignKey("ChapterId")]
        public virtual Chapter? Chapter { get; set; }

        [ForeignKey("LessonId")]
        public virtual Lesson? Lesson { get; set; }

        [ForeignKey("TextbookId")]
        public virtual TextBook? Textbook { get; set; }

        [ForeignKey("TemplateQuestionId")]
        public virtual Question? TemplateQuestion { get; set; }

        public virtual ICollection<Question> ClonedQuestions { get; set; } = new List<Question>();
        public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
        public virtual ICollection<Solution> Solutions { get; set; } = new List<Solution>();
        public virtual ICollection<QuestionComment> QuestionComments { get; set; } = new List<QuestionComment>();
        public virtual ICollection<QuestionReport> QuestionReports { get; set; } = new List<QuestionReport>();
        public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
        public virtual ICollection<Chatbot> Chatbots { get; set; } = new List<Chatbot>();
    }
}