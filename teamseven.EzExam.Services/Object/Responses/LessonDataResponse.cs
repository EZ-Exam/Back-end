using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Responses
{
    public class LessonDataResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ChapterId { get; set; }
        public string? Document { get; set; }
        public string? DocumentType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}