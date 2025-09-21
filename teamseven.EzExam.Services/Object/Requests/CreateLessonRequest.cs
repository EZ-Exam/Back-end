using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class CreateLessonRequest
    {
        [Required(ErrorMessage = "Lesson name is required.")]
        [StringLength(255, ErrorMessage = "Lesson name must be at most 255 characters.")]
        public string Name { get; set; }

        public int ChapterId { get; set; }

        [StringLength(5000, ErrorMessage = "Document cannot exceed 5000 characters.")]
        public string? Document { get; set; }

        [StringLength(50, ErrorMessage = "Document type cannot exceed 50 characters.")]
        public string? DocumentType { get; set; }
    }
}
