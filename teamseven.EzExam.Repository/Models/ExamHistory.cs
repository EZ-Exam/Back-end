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
        [Column("UserId")]
        public int UserId { get; set; }

        [Required]
        [Column("Score")]
        public decimal Score { get; set; } = 0;

        [Required]
        [Column("CorrectCount")]
        public int CorrectCount { get; set; } = 0;

        [Required]
        [Column("IncorrectCount")]
        public int IncorrectCount { get; set; } = 0;

        [Required]
        [Column("UnansweredCount")]
        public int UnansweredCount { get; set; } = 0;

        [Required]
        [Column("TotalQuestions")]
        public int TotalQuestions { get; set; } = 0;

        [Required]
        [Column("SubmittedAt")]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("TimeTaken")]
        public int TimeTaken { get; set; } = 0; // seconds

        [Column("Answers")]
        [MaxLength(10000)]
        public string? Answers { get; set; } // JSON array of answer objects

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}