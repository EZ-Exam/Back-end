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

        public int AnswerCount { get; set; }
        public int CommentCount { get; set; }

        public bool IsAnsweredByCurrentUser { get; set; }

        public string Type { get; set; } = "multiple-choice";
        public string? DifficultyLevel { get; set; }
    }
}
