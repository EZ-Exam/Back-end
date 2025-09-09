using System;

namespace teamseven.EzExam.Repository.Dtos
{
    public class UserSubscriptionResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubscriptionTypeId { get; set; }
        public string PaymentStatus { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentGatewayTransactionId { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UserEmail { get; set; }
        public string SubscriptionName { get; set; }
    }
}
