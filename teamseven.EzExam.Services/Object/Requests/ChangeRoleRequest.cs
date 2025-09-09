using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class ChangeRoleRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        public string RoleName { get; set; }

        [Required(ErrorMessage = "Secret key is required")]
        public string SecretKey { get; set; }
    }
}
