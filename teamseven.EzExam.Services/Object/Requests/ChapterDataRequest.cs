using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class ChapterDataRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Chapter name is required.")]
        [StringLength(255, ErrorMessage = "Chapter name must be at most 255 characters.")]
        public string Name { get; set; }

        public int SemesterId { get; set; }
    }
}
