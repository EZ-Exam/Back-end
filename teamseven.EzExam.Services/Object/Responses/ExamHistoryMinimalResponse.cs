namespace teamseven.EzExam.Services.Object.Responses
{
    public class ExamHistoryMinimalResponse
    {
        public int ExamId { get; set; }
        public int UserId { get; set; }
        public decimal Score { get; set; }
        public int CorrectCount { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime SubmittedAt { get; set; }
        public int TimeTaken { get; set; }
        
        // Thông tin từ Exam để tự động track
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int? GradeId { get; set; }
        public string GradeName { get; set; } = string.Empty;
        public int? ChapterId { get; set; }
        public string ChapterName { get; set; } = string.Empty;
        public int? LessonId { get; set; }
        public string LessonName { get; set; } = string.Empty;
    }
}
