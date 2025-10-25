namespace teamseven.EzExam.Services.Object.Responses
{
    public class GenerateAIExamResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<AIQuestionResponse> Questions { get; set; } = new List<AIQuestionResponse>();
        public AIExamMetadata Metadata { get; set; } = new AIExamMetadata();
    }

    public class AIQuestionResponse
    {
        public int QuestionId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string DifficultyLevel { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public string CorrectAnswer { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public string? Formula { get; set; }
        public string? QuestionSource { get; set; }
        public int GradeId { get; set; }
        public string GradeName { get; set; } = string.Empty;
        public int LessonId { get; set; }
        public string LessonName { get; set; } = string.Empty;
        public string AIReasoning { get; set; } = string.Empty; // Lý do AI chọn câu hỏi này
    }

    public class AIExamMetadata
    {
        public int ExamId { get; set; } // ID của exam đã lưu trong DB
        public int TotalQuestions { get; set; }
        public string Mode { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string AIModel { get; set; } = string.Empty;
        public int TokensUsed { get; set; }
        public double ProcessingTimeSeconds { get; set; }
        public string Analysis { get; set; } = string.Empty; // Phân tích từ AI
        public Dictionary<string, int> DifficultyDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> SubjectDistribution { get; set; } = new Dictionary<string, int>();
    }
}
