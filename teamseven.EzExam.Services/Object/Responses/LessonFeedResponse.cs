using System;

namespace teamseven.EzExam.Services.Object.Responses
{
    public class LessonFeedResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ChapterId { get; set; }
        public string? ChapterName { get; set; }
        public int? SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Metadata
        public int QuestionCount { get; set; }
        public int ExamCount { get; set; }
        public int AttemptCount { get; set; }
        public decimal AverageScore { get; set; }
        public bool IsAttemptedByCurrentUser { get; set; }
    }
}
