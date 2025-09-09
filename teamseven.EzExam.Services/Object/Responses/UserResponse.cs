using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public decimal? Balance { get; set; }
        public string? FullName { get; set; }
        public string AvatarUrl { get; set; }
        public string PhoneNumber { get; set; }
        public int RoleId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
