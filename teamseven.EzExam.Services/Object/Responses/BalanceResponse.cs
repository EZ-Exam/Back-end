namespace teamseven.EzExam.Services.Object.Responses
{
    public class BalanceResponse
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public decimal PreviousBalance { get; set; }
        public decimal AddedAmount { get; set; }
        public decimal NewBalance { get; set; }
        public string? Description { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
