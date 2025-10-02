namespace teamseven.EzExam.Services.Object.Responses
{
    public class StudentQuestionAttemptResponse
    {
        public int Id { get; set; }
        public int StudentQuizHistoryId { get; set; }
        public int QuestionId { get; set; }
        public int UserId { get; set; }
        public int? SelectedAnswerId { get; set; }
        public string? UserAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public string DifficultyLevel { get; set; } = string.Empty;
        public int TimeSpent { get; set; }
        public string? Topic { get; set; }
        public int? ChapterId { get; set; }
        public int? LessonId { get; set; }
        public int QuestionOrder { get; set; }
        public int? ConfidenceLevel { get; set; }
        public bool IsMarkedForReview { get; set; }
        public bool IsSkipped { get; set; }
        public int AnswerChangeCount { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties for display
        public string? QuestionContent { get; set; }
        public string? QuestionType { get; set; }
        public string? SelectedAnswerContent { get; set; }
        public string? CorrectAnswerContent { get; set; }
        public string? ChapterName { get; set; }
        public string? LessonName { get; set; }
        public string? Explanation { get; set; }
    }
}
