namespace teamseven.EzExam.Services.Object.Responses
{
    public class SolutionDataResponse
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public string Explanation { get; set; }
        public int CreatedByUserId { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] VideoData { get; set; }
        public string VideoContentType { get; set; }

    }
}
