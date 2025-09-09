using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Helpers;

namespace teamseven.EzExam.Services.Object.Responses
{
    public class GradeDataResponse
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string EncodedId => IdHelper.EncodeId(Id);

        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
