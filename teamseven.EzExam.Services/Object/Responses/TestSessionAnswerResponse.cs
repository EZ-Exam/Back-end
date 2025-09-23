namespace teamseven.EzExam.Services.Object.Responses
{
    public class TestSessionAnswerResponse
    {
        public int Id { get; set; }
        public int TestSessionId { get; set; }
        public int QuestionId { get; set; }
        public int? SelectedAnswerId { get; set; }
        public string? UserAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public int TimeSpent { get; set; }
        public DateTime AnsweredAt { get; set; }
        public bool IsMarkedForReview { get; set; }
        public int? ConfidenceLevel { get; set; }
        public int AnswerSequence { get; set; }
        public bool IsChanged { get; set; }
        public int ChangeCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
