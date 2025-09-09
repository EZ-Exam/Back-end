namespace teamseven.EzExam.Services.Object.Responses
{
    public class ExamDataResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LessonId { get; set; }
        public int ExamTypeId { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
