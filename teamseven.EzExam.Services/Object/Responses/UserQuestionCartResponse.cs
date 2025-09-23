namespace teamseven.EzExam.Services.Object.Responses
{
    public class UserQuestionCartResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public bool IsSelected { get; set; }
        public string? UserNotes { get; set; }
        public string? DifficultyPreference { get; set; }
        public int? SubjectId { get; set; }
        public int? ChapterId { get; set; }
        public int? LessonId { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
