using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class UpdateQuestionReportRequest
    {
        [Required(ErrorMessage = "Report ID is required.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Reason is required.")]
        [StringLength(1000, ErrorMessage = "Reason must be at most 1000 characters.")]
        public string Reason { get; set; }

        public bool IsHandled { get; set; }
    }
}
