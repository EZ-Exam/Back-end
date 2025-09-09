using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Helpers;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class GradeDataRequest
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        public int GetDecodedId() => IdHelper.DecodeId(Id);
    }
}
