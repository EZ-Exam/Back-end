using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Responses
{
    public class QuestionReportDataResponse
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int ReportedByUserId { get; set; }
        public string Reason { get; set; }
        public DateTime ReportDate { get; set; }
        public bool IsHandled { get; set; }
    }
}
