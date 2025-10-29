namespace teamseven.EzExam.Services.Object.Requests
{
    public class QuestionSearchRequest
    {
        public string? Content { get; set; }
        
        /// <summary>
        /// ID độ khó (1=EASY, 2=MEDIUM, 3=HARD). Nếu null → Lấy tất cả độ khó.
        /// </summary>
        public int? DifficultyLevelId { get; set; }
        
        // Hỗ trợ lọc theo nhiều giá trị - Hierarchy: Grade → Subject → Chapter → Lesson
        public List<int>? GradeIds { get; set; }
        public List<int>? SubjectIds { get; set; }
        public List<int>? ChapterIds { get; set; }
        public List<int>? LessonIds { get; set; }
    }
}
