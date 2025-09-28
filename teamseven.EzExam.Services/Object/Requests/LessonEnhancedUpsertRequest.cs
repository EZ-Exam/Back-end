using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class LessonEnhancedUpsertRequest
    {
        [Required, StringLength(200)]
        public string title { get; set; } = "";

        [StringLength(1000)]
        public string? description { get; set; }

        [Required]
        public string subjectId { get; set; } = "";

        [StringLength(500)]
        public string? pdfUrl { get; set; }

        public List<string> questions { get; set; } = new();
    }
}
