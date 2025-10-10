namespace teamseven.EzExam.Services.Object.Responses
{
    public class QuestionSimpleResponse
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? DifficultyLevel { get; set; }
        public int? GradeId { get; set; }
        public string? GradeName { get; set; }
        public int? LessonId { get; set; }
        public string? LessonName { get; set; }
    }
}
