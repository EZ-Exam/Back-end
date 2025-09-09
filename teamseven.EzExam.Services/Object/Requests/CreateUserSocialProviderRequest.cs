namespace teamseven.EzExam.Services.Object.Requests
{
    public class CreateUserSocialProviderRequest
    {
        public int UserId { get; set; }
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
        public string Email { get; set; }
        public string ProfileUrl { get; set; }
    }
}
