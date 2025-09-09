using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class ExamQuestionRequest
    {
        [Required(ErrorMessage = "ExamId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ExamId must be a positive number")]
        public int ExamId { get; set; }

        [Required(ErrorMessage = "QuestionId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "QuestionId must be a positive number")]
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Order is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Order must be a positive number")]
        public int Order { get; set; }
    }
}
