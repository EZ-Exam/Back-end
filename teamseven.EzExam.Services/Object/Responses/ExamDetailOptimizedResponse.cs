using System;
using System.Collections.Generic;

namespace teamseven.EzExam.Services.Object.Responses
{
    public class ExamDetailOptimizedResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? SubjectId { get; set; }
        public int? LessonId { get; set; }
        public string? ExamTypeName { get; set; }
        public int CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Metadata
        public int TotalQuestions { get; set; }
        public List<int> QuestionIds { get; set; } = new List<int>();
        public int AttemptCount { get; set; }
        public decimal AverageScore { get; set; }
        public bool IsAttemptedByCurrentUser { get; set; }

        public int? TimeLimit { get; set; }
        public int? Duration { get; set; }
    }
}
