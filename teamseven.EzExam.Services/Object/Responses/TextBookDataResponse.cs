using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Responses
{
    public class TextBookDataResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GradeId { get; set; }
        public int? SubjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
