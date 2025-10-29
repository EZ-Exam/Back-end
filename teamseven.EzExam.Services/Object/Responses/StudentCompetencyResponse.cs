namespace teamseven.EzExam.Services.Object.Responses
{
    /// <summary>
    /// Response DTO for AI recommendation system to get student competency data
    /// </summary>
    public class StudentCompetencyResponse
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public int? SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public int? GradeId { get; set; }
        public string? GradeName { get; set; }

        // Overall Performance Metrics
        public decimal OverallAverageScore { get; set; }
        public decimal OverallAccuracy { get; set; }
        public string? CurrentPerformanceLevel { get; set; } // EXCELLENT, GOOD, AVERAGE, NEEDS_IMPROVEMENT, POOR
        public string? ImprovementTrend { get; set; } // IMPROVING, STABLE, DECLINING
        public decimal? TrendPercentage { get; set; }

        // Recent Performance (Last 5 Quizzes)
        public List<RecentQuizPerformance> RecentQuizzes { get; set; } = new List<RecentQuizPerformance>();
        public decimal RecentAverageScore { get; set; }
        public decimal RecentAverageTime { get; set; }
        public decimal ConsistencyScore { get; set; }
        public decimal LearningVelocity { get; set; }

        // Difficulty Analysis
        public DifficultyProfileData? DifficultyProfile { get; set; }
        public string? RecommendedDifficulty { get; set; }

        // Topic Analysis
        public List<TopicPerformanceData> StrongTopics { get; set; } = new List<TopicPerformanceData>();
        public List<TopicPerformanceData> WeakTopics { get; set; } = new List<TopicPerformanceData>();

        // Learning Characteristics
        public decimal ConfidenceLevel { get; set; }
        public decimal TimeManagementScore { get; set; }
        public decimal? PredictedNextScore { get; set; }

        // Recommendations for AI
        public List<string> RecommendedTopics { get; set; } = new List<string>();
        public List<string> AreasForImprovement { get; set; } = new List<string>();
        public string? OptimalQuestionDifficulty { get; set; }
        public int? RecommendedQuestionCount { get; set; }
        public int? RecommendedTimeLimit { get; set; }

        public DateTime LastAnalysisDate { get; set; }
        public DateTime? LastQuizDate { get; set; }
    }

    public class RecentQuizPerformance
    {
        public int QuizHistoryId { get; set; }
        public int ExamId { get; set; }
        public string? ExamName { get; set; }
        public decimal Score { get; set; }
        public int TimeSpent { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public DateTime CompletedAt { get; set; }
        public string? PerformanceRating { get; set; }
        public List<string>? TopicsPerformed { get; set; }
    }

    public class DifficultyProfileData
    {
        public decimal EasyAccuracy { get; set; }
        public decimal MediumAccuracy { get; set; }
        public decimal HardAccuracy { get; set; }
        public decimal EasyAverageTime { get; set; }
        public decimal MediumAverageTime { get; set; }
        public decimal HardAverageTime { get; set; }
    }

    public class TopicPerformanceData
    {
        public string TopicName { get; set; } = string.Empty;
        public decimal Accuracy { get; set; }
        public decimal AverageTime { get; set; }
        public int QuestionCount { get; set; }
        public string? PerformanceLevel { get; set; }
    }
}
