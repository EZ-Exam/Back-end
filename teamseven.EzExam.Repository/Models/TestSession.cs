using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("test_sessions")]
    public class TestSession
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        [Required]
        [Column("ExamId")]
        public int ExamId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("SessionStatus")]
        public string SessionStatus { get; set; } = "NOT_STARTED"; // NOT_STARTED, IN_PROGRESS, COMPLETED, ABANDONED, TIMEOUT

        [Column("StartedAt")]
        public DateTime? StartedAt { get; set; }

        [Column("CompletedAt")]
        public DateTime? CompletedAt { get; set; }

        [Column("TimeSpent")]
        public int TimeSpent { get; set; } = 0; // seconds

        [Column("TotalScore")]
        public decimal? TotalScore { get; set; } // percentage

        [Column("CorrectAnswers")]
        public int CorrectAnswers { get; set; } = 0;

        [Column("TotalQuestions")]
        public int TotalQuestions { get; set; } = 0;

        [Column("IsPassed")]
        public bool? IsPassed { get; set; } // null = not evaluated, true = passed, false = failed

        [Column("PassingScore")]
        public decimal? PassingScore { get; set; } // percentage - optional passing score

        [Column("SessionData")]
        [MaxLength(10000)]
        public string? SessionData { get; set; } // JSON: answers, time per question, etc.

        [Column("DeviceInfo")]
        [MaxLength(500)]
        public string? DeviceInfo { get; set; } // JSON: browser, OS, IP, etc.

        [Column("IsCheatingDetected")]
        public bool IsCheatingDetected { get; set; } = false;

        [Column("CheatingDetails")]
        [MaxLength(1000)]
        public string? CheatingDetails { get; set; } // JSON: details of cheating detection

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; } = null!;

        public virtual ICollection<TestSessionAnswer> TestSessionAnswers { get; set; } = new List<TestSessionAnswer>();
    }
}
