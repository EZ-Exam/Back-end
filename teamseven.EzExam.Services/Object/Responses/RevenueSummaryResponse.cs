using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Responses
{
    public class RevenueSummaryResponse
    {
        public decimal TotalAmount { get; set; }
        public int CompletedCount { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
