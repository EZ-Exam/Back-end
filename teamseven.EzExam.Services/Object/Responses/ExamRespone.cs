using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Responses
{
    public class ExamResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? LessonId { get; set; }
        public int ExamTypeId { get; set; }
        public int CreatedByUserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string CreatedByUserName { get; set; }
        public string ExamTypeName { get; set; }
        public string LessonName { get; set; }

        public int QuestionCount { get; set; }
        public int HistoryCount { get; set; }
    }
}
