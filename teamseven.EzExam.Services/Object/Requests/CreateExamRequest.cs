namespace teamseven.EzExam.Services.Object.Requests
{
    public class CreateExamRequest
    {
        public string Name { get; set; }
        public int LessonId { get; set; }
        public int ExamTypeId { get; set; }
        public int CreatedByUserId { get; set; }
    }
}
