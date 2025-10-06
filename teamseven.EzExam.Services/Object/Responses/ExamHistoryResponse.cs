namespace teamseven.EzExam.Services.Object.Responses
{
    public class ExamHistoryResponse
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int UserId { get; set; }
        public decimal Score { get; set; }
        public int CorrectCount { get; set; }
        public int IncorrectCount { get; set; }
        public int UnansweredCount { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime SubmittedAt { get; set; }
        public int TimeTaken { get; set; }
        public List<AnswerDetailResponse>? Answers { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class AnswerDetailResponse
    {
        public string QuestionId { get; set; } = string.Empty;
        public string? SelectedAnswer { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int TimeSpent { get; set; }
       
        public string ContentQuestion { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public string? Explanation { get; set; }
        public string? ImageUrl { get; set; }
        public string? Formula { get; set; }
    }
}
