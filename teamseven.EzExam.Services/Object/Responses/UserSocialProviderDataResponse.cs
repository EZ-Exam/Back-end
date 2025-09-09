namespace teamseven.EzExam.Services.Object.Responses
{
    public class UserSocialProviderDataResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
        public string Email { get; set; }
        public string ProfileUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
