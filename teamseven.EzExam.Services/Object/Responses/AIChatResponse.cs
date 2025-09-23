namespace teamseven.EzExam.Services.Object.Responses
{
    public class AIChatResponse
    {
        public string Response { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
