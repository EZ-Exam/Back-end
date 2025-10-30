using System;
using System.Collections.Generic;

namespace teamseven.EzExam.Services.Object.Responses
{
    public class LessonDetailOptimizedResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ChapterId { get; set; }
        public int? SubjectId { get; set; }
        public string? ChapterName { get; set; }
        public string? SubjectName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Lightweight collections
        public List<int> QuestionIds { get; set; } = new List<int>();
        public List<int> ExamIds { get; set; } = new List<int>();

        // Metadata
        public int QuestionCount { get; set; }
        public int ExamCount { get; set; }
        public int AttemptCount { get; set; }
        public decimal AverageScore { get; set; }
        public bool IsAttemptedByCurrentUser { get; set; }
    }
}
