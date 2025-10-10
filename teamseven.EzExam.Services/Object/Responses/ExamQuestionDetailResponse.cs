namespace teamseven.EzExam.Services.Object.Responses
{
    public class ExamQuestionDetailResponse
    {
        public int Id { get; set; }
        public string ContentQuestion { get; set; } = string.Empty;
        public string? CorrectAnswer { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public string? Explanation { get; set; }
        public string? ImageUrl { get; set; }
        public string? Formula { get; set; }
    }
}