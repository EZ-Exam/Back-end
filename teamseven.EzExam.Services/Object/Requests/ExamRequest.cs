using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class ExamRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "SubjectId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "SubjectId must be a positive number")]
        public int SubjectId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "LessonId must be a positive number")]
        public int? LessonId { get; set; }

        [Required(ErrorMessage = "ExamTypeId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ExamTypeId must be a positive number")]
        public int ExamTypeId { get; set; }

        [Required(ErrorMessage = "CreatedByUserId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "CreatedByUserId must be a positive number")]
        public int CreatedByUserId { get; set; }
    }
}
