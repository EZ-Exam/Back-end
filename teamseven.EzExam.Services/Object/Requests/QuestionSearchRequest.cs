namespace teamseven.EzExam.Services.Object.Requests
{
    public class QuestionSearchRequest
    {
        public string? Content { get; set; }
        public string? DifficultyLevel { get; set; }
        public int? GradeId { get; set; }
        public int? LessonId { get; set; }
    }
}
