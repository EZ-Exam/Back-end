using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class CreateQuestionReportRequest
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        public int ReportedByUserId { get; set; }

        [Required]
        [MaxLength(1000, ErrorMessage = "Reason must be at most 1000 characters.")]
        public string Reason { get; set; }
    }
}
