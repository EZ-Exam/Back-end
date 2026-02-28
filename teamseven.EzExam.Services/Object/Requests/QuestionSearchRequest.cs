namespace teamseven.EzExam.Services.Object.Requests
{
    public class QuestionSearchRequest
    {
        public string? Content { get; set; }
        
        public int? DifficultyLevelId { get; set; }
        
        public List<int>? GradeIds { get; set; }
        public List<int>? SubjectIds { get; set; }
        public List<int>? ChapterIds { get; set; }
        public List<int>? LessonIds { get; set; }
    }
}
