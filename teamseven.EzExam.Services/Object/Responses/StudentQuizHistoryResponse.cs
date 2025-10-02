namespace teamseven.EzExam.Services.Object.Responses
{
    public class StudentQuizHistoryResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ExamId { get; set; }
        public int? TestSessionId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int TimeSpent { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers { get; set; }
        public int SkippedQuestions { get; set; }
        public decimal TotalScore { get; set; }
        public decimal? PassingScore { get; set; }
        public bool? IsPassed { get; set; }
        public string QuizStatus { get; set; } = string.Empty;
        public decimal AverageTimePerQuestion { get; set; }
        public string? DifficultyBreakdown { get; set; }
        public string? TopicPerformance { get; set; }
        public string? WeakAreas { get; set; }
        public string? StrongAreas { get; set; }
        public string? ImprovementAreas { get; set; }
        public string? PerformanceRating { get; set; }
        public decimal? ComparedToPrevious { get; set; }
        public string? DeviceInfo { get; set; }
        public bool IsCheatingDetected { get; set; }
        public string? CheatingDetails { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties for display
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? ExamName { get; set; }
        public string? SubjectName { get; set; }
        public string? ExamTypeName { get; set; }
    }
}
