using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class Gemini15ChatRequest
    {
        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; } = string.Empty;
    }

    public class Gemini25ChatRequest
    {
        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; } = string.Empty;
    }
}
