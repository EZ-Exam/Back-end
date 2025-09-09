using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace teamseven.EzExam.Repository.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(255)]
        [Column("PasswordHash")]
        public string? PasswordHash { get; set; }

        [MaxLength(255)]
        [Column("FullName")]
        public string? FullName { get; set; }

        [MaxLength(1024)]
        [Column("AvatarUrl")]
        public string? AvatarUrl { get; set; }

        [MaxLength(20)]
        [Column("PhoneNumber")]
        public string? PhoneNumber { get; set; }

        [Required]
        [Column("RoleId")]
        public int RoleId { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("EmailVerifiedAt")]
        public DateTime? EmailVerifiedAt { get; set; }

        [Column("IsPremium")]
        public bool? IsPremium { get; set; }

        [Column("LastLoginAt")]
        public DateTime? LastLoginAt { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedBy")]
        public int? UpdatedBy { get; set; }

        [Column("Balance")]
        public decimal? Balance { get; set; }

        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;

        [ForeignKey("UpdatedBy")]
        public virtual User? UpdatedByNavigation { get; set; }

        public virtual ICollection<User> InverseUpdatedByNavigation { get; set; } = new List<User>();
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
        public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
        public virtual ICollection<Solution> Solutions { get; set; } = new List<Solution>();
        public virtual ICollection<QuestionComment> QuestionComments { get; set; } = new List<QuestionComment>();
        public virtual ICollection<QuestionReport> QuestionReports { get; set; } = new List<QuestionReport>();
        public virtual ICollection<SolutionReport> SolutionReports { get; set; } = new List<SolutionReport>();
        public virtual ICollection<Chatbot> Chatbots { get; set; } = new List<Chatbot>();
        public virtual ICollection<UserSocialProvider> UserSocialProviders { get; set; } = new List<UserSocialProvider>();
        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
        public virtual ICollection<ExamHistory> ExamHistories { get; set; } = new List<ExamHistory>();
    }
}