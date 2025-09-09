using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class ExamHistoryRequest
    {
        [Required(ErrorMessage = "ExamId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ExamId must be a positive number")]
        public int ExamId { get; set; }

        [Required(ErrorMessage = "ActionByUserId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ActionByUserId must be a positive number")]
        public int ActionByUserId { get; set; }

        [Required(ErrorMessage = "Action is required")]
        [StringLength(50, ErrorMessage = "Action cannot exceed 50 characters")]
        public string Action { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }
    }
}
