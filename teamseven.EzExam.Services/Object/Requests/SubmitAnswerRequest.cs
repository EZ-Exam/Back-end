using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class SubmitAnswerRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Test session ID must be a positive integer.")]
        public int TestSessionId { get; set; }

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
        [Range(0, int.MaxValue, ErrorMessage = "Time spent must be a non-negative integer.")]
        public int TimeSpent { get; set; }

        public bool IsMarkedForReview { get; set; } = false;

        [Range(1, 5, ErrorMessage = "Confidence level must be between 1 and 5.")]
        public int? ConfidenceLevel { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Answer sequence must be a positive integer.")]
        public int AnswerSequence { get; set; }

        public bool IsChanged { get; set; } = false;

        [Range(0, int.MaxValue, ErrorMessage = "Change count must be a non-negative integer.")]
        public int ChangeCount { get; set; } = 0;
    }
}
