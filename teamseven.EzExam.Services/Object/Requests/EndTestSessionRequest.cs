using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class EndTestSessionRequest
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Time spent must be a non-negative integer.")]
        public int TimeSpent { get; set; }

        [Range(0, 100, ErrorMessage = "Total score must be between 0 and 100.")]
        public decimal? TotalScore { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Correct answers must be a non-negative integer.")]
        public int CorrectAnswers { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Total questions must be a non-negative integer.")]
        public int TotalQuestions { get; set; }

        [StringLength(10000, ErrorMessage = "Session data cannot exceed 10000 characters.")]
        public string? SessionData { get; set; }
    }
}
