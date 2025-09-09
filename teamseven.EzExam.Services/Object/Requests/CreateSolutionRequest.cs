namespace teamseven.EzExam.Services.Object.Requests
{
    public class CreateSolutionRequest
    {
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public string Explanation { get; set; }
        public int CreatedByUserId { get; set; }
    }
}
