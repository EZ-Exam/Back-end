using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Responses
{
    public class ExamQuestionResponse
    {
        public int Id { get; set; }

        public int ExamId { get; set; }

        public int QuestionId { get; set; }

        public int Order { get; set; }

        public DateTime CreatedAt { get; set; }

        public string ExamName { get; set; }

        public string QuestionContent { get; set; }
    }
}
