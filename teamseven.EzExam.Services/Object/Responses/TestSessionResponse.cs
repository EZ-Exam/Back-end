namespace teamseven.EzExam.Services.Object.Responses
{
    public class TestSessionResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ExamId { get; set; }
        public string SessionStatus { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? TimeSpent { get; set; }
        public decimal? TotalScore { get; set; }
        public int? CorrectAnswers { get; set; }
        public int? TotalQuestions { get; set; }
        public bool? IsPassed { get; set; }
        public decimal? PassingScore { get; set; }
        public string? SessionData { get; set; }
        public string? DeviceInfo { get; set; }
        public bool? IsCheatingDetected { get; set; }
        public string? CheatingDetails { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
