using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class AISolveRequest
    {
        [Required]
        [StringLength(10000, MinimumLength = 1)]
        public string Input { get; set; } = string.Empty;

        [StringLength(5000)]
        public string? DefaultPrompt { get; set; }

        [Required]
        [StringLength(50)]
        public string AIModel { get; set; } = "gpt-4o";

        [StringLength(20)]
        public string Provider { get; set; } = "openai";
    }
}
