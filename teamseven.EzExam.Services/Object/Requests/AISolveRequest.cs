using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class AISolveRequest
    {
        [Required]
        [StringLength(10000, MinimumLength = 1)]
        public string Input { get; set; } = string.Empty;

        [StringLength(5000)]
        public string? DefaultPrompt { get; set; } // Optional custom prompt

        [Required]
        [StringLength(50)]
        public string AIModel { get; set; } = "gpt-4o"; // AI model (e.g., gpt-4o, gpt-3.5-turbo, gemini-2.5-flash, etc.)

        [StringLength(20)]
        public string Provider { get; set; } = "openai"; // Provider type for API format handling
    }
}
