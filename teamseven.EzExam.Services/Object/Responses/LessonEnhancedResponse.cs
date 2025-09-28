using System;
using System.Collections.Generic;

namespace teamseven.EzExam.Services.Object.Responses
{
    public class LessonEnhancedResponse
    {
        public string id { get; set; } = "";
        public string title { get; set; } = "";
        public string? description { get; set; }
        public string subjectId { get; set; } = "";
        public string? pdfUrl { get; set; }
        public List<string> questions { get; set; } = new();
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
}
