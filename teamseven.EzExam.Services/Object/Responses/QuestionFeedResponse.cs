using System;

namespace teamseven.EzExam.Services.Object.Responses
{
    public class QuestionFeedResponse
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int? LessonId { get; set; }
        public string? LessonName { get; set; }
        public string? ChapterName { get; set; }

        public int CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }

        // Metadata counts (only numbers, no full collections)
        public int AnswerCount { get; set; }
        public int CommentCount { get; set; }

        // If current user has any interaction (attempted/answered)
        public bool IsAnsweredByCurrentUser { get; set; }

        public string Type { get; set; } = "multiple-choice";
        public string? DifficultyLevel { get; set; }
    }
}
