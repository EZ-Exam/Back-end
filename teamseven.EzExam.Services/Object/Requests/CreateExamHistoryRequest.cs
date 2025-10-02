using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class CreateExamHistoryRequest
    {
        [Required]
        public string ExamId { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Range(0, 100)]
        public decimal Score { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int CorrectCount { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int IncorrectCount { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int UnansweredCount { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int TotalQuestions { get; set; }

        [Required]
        public DateTime SubmittedAt { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int TimeTaken { get; set; } // seconds

        public List<AnswerDetail>? Answers { get; set; }
    }

    public class AnswerDetail
    {
        [Required]
        public string QuestionId { get; set; } = string.Empty;

        public string? SelectedAnswer { get; set; }

        [Required]
        public string CorrectAnswer { get; set; } = string.Empty;

        [Required]
        public bool IsCorrect { get; set; }

        [Range(0, int.MaxValue)]
        public int TimeSpent { get; set; } // seconds
    }
}
