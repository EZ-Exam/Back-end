using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class GenerateAIExamRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "UserId must be a positive integer")]
        public int UserId { get; set; }

        [Required]
        [Range(1, 50, ErrorMessage = "QuestionCount must be between 1 and 50")]
        public int QuestionCount { get; set; }

        [Required]
        [RegularExpression("^(review|advanced)$", ErrorMessage = "Mode must be either 'review' or 'advanced'")]
        public string Mode { get; set; } = string.Empty; // "review" hoặc "advanced"

        [Range(1, 5, ErrorMessage = "HistoryCount must be between 1 and 5")]
        public int HistoryCount { get; set; } = 5; // Số lịch sử làm bài gần nhất

        // Optional filters
        public int? GradeId { get; set; }
        public int? LessonId { get; set; }
        public string? DifficultyLevel { get; set; }
        public string? Subject { get; set; }
    }
}
