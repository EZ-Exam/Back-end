namespace teamseven.EzExam.Services.Object.Responses
{
    public class QuestionMinimalResponse
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string DifficultyLevel { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public string LessonName { get; set; } = string.Empty;
    }
}
