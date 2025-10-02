namespace teamseven.EzExam.Services.Object.Responses
{
    public class StudentPerformanceSummaryResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? SubjectId { get; set; }
        public int? GradeId { get; set; }
        public int TotalQuizzesCompleted { get; set; }
        public int RecentQuizzesCount { get; set; }
        public string? RecentQuizIds { get; set; }
        public decimal AverageScore { get; set; }
        public decimal AverageTimePerQuiz { get; set; }
        public decimal AverageTimePerQuestion { get; set; }
        public decimal OverallAccuracy { get; set; }
        public string? ImprovementTrend { get; set; }
        public decimal? TrendPercentage { get; set; }
        public string? StrongTopics { get; set; }
        public string? WeakTopics { get; set; }
        public string? DifficultyProfile { get; set; }
        public string? RecommendedDifficulty { get; set; }
        public decimal LearningVelocity { get; set; }
        public decimal ConsistencyScore { get; set; }
        public decimal? PredictedNextScore { get; set; }
        public decimal ConfidenceLevel { get; set; }
        public decimal TimeManagementScore { get; set; }
        public DateTime? LastQuizDate { get; set; }
        public DateTime LastAnalysisDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties for display
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? SubjectName { get; set; }
        public string? GradeName { get; set; }

        // Additional computed properties
        public List<StudentQuizHistoryResponse>? RecentQuizzes { get; set; }
        public string? PerformanceInsights { get; set; }
        public List<string>? Recommendations { get; set; }
    }
}
