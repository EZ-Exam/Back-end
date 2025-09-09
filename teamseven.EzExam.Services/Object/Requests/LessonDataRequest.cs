using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class LessonDataRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Lesson name is required.")]
        [StringLength(255, ErrorMessage = "Lesson name must be at most 255 characters.")]
        public string Name { get; set; }

        public int ChapterId { get; set; }
    }
}
