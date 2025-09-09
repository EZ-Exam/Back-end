using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Responses
{
    public class ExamHistoryResponseDto
    {
        public int Id { get; set; }

        public int ExamId { get; set; }

        public int ActionByUserId { get; set; }

        public string Action { get; set; }

        public string Description { get; set; }

        public DateTime ActionDate { get; set; }

        public string ExamName { get; set; }

        public string ActionByUserName { get; set; }
    }
}
