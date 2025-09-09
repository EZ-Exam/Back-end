using System;

namespace teamseven.EzExam.Repository.Dtos
{
    public class SubscriptionTypeResponse
    {
        public int Id { get; set; }
        public string SubscriptionCode { get; set; }
        public string SubscriptionName { get; set; }
        public decimal SubscriptionPrice { get; set; }
        public int? DurationInDays { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
