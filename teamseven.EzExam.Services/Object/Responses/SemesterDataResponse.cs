using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Responses
{
    public class SemesterDataResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int GradeId { get; set; }

        public string GradeName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
