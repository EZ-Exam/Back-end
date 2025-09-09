using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class UserSubscriptionRequest
    {
        public int UserId { get; set; }
        public int SubscriptionTypeId { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentGatewayTransactionId { get; set; }
    }
}
