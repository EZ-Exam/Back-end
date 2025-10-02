using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class StudentQuestionAttemptRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Question ID must be a positive integer.")]
        public int QuestionId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selected answer ID must be a positive integer.")]
        public int? SelectedAnswerId { get; set; }

        [StringLength(5000, ErrorMessage = "User answer cannot exceed 5000 characters.")]
        public string? UserAnswer { get; set; }

        [Required]
        public bool IsCorrect { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Difficulty level cannot exceed 20 characters.")]
        public string DifficultyLevel { get; set; } = "MEDIUM";

        [Range(0, int.MaxValue, ErrorMessage = "Time spent must be a non-negative integer.")]
        public int TimeSpent { get; set; } = 0;

        [StringLength(200, ErrorMessage = "Topic cannot exceed 200 characters.")]
        public string? Topic { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Chapter ID must be a positive integer.")]
        public int? ChapterId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Lesson ID must be a positive integer.")]
        public int? LessonId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Question order must be a positive integer.")]
        public int QuestionOrder { get; set; } = 0;

        [Range(1, 5, ErrorMessage = "Confidence level must be between 1 and 5.")]
        public int? ConfidenceLevel { get; set; }

        public bool IsMarkedForReview { get; set; } = false;

        public bool IsSkipped { get; set; } = false;

        [Range(0, int.MaxValue, ErrorMessage = "Answer change count must be a non-negative integer.")]
        public int AnswerChangeCount { get; set; } = 0;
    }
}
