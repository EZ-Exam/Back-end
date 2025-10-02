using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class CreateStudentQuizHistoryRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer.")]
        public int UserId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Exam ID must be a positive integer.")]
        public int ExamId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Test Session ID must be a positive integer.")]
        public int? TestSessionId { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Time spent must be a non-negative integer.")]
        public int TimeSpent { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Total questions must be a positive integer.")]
        public int TotalQuestions { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Correct answers must be a non-negative integer.")]
        public int CorrectAnswers { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Incorrect answers must be a non-negative integer.")]
        public int IncorrectAnswers { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Skipped questions must be a non-negative integer.")]
        public int SkippedQuestions { get; set; } = 0;

        [Required]
        [Range(0, 100, ErrorMessage = "Total score must be between 0 and 100.")]
        public decimal TotalScore { get; set; }

        [Range(0, 100, ErrorMessage = "Passing score must be between 0 and 100.")]
        public decimal? PassingScore { get; set; }

        public bool? IsPassed { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Quiz status cannot exceed 50 characters.")]
        public string QuizStatus { get; set; } = "COMPLETED";

        [StringLength(1000, ErrorMessage = "Difficulty breakdown cannot exceed 1000 characters.")]
        public string? DifficultyBreakdown { get; set; }

        [StringLength(2000, ErrorMessage = "Topic performance cannot exceed 2000 characters.")]
        public string? TopicPerformance { get; set; }

        [StringLength(1000, ErrorMessage = "Weak areas cannot exceed 1000 characters.")]
        public string? WeakAreas { get; set; }

        [StringLength(1000, ErrorMessage = "Strong areas cannot exceed 1000 characters.")]
        public string? StrongAreas { get; set; }

        [StringLength(1000, ErrorMessage = "Improvement areas cannot exceed 1000 characters.")]
        public string? ImprovementAreas { get; set; }

        [StringLength(20, ErrorMessage = "Performance rating cannot exceed 20 characters.")]
        public string? PerformanceRating { get; set; }

        [StringLength(500, ErrorMessage = "Device info cannot exceed 500 characters.")]
        public string? DeviceInfo { get; set; }

        [StringLength(10000, ErrorMessage = "Session data cannot exceed 10000 characters.")]
        public string? SessionData { get; set; }

        public bool IsCheatingDetected { get; set; } = false;

        [StringLength(1000, ErrorMessage = "Cheating details cannot exceed 1000 characters.")]
        public string? CheatingDetails { get; set; }

        // Chi tiết từng câu hỏi học sinh làm
        public List<StudentQuestionAttemptRequest> QuestionAttempts { get; set; } = new List<StudentQuestionAttemptRequest>();
    }
}
