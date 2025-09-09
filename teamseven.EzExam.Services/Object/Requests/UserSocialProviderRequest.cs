using System;
using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Repository.Dtos
{
    public class UserSocialProviderRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Provider name is required.")]
        [StringLength(100, ErrorMessage = "Provider name cannot exceed 100 characters.")]
        public string ProviderName { get; set; }

        [Required(ErrorMessage = "Provider ID is required.")]
        [StringLength(255, ErrorMessage = "Provider ID cannot exceed 255 characters.")]
        public string ProviderId { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string Email { get; set; }

        [Url(ErrorMessage = "Invalid profile URL format.")]
        [StringLength(2048, ErrorMessage = "Profile URL is too long.")]
        public string ProfileUrl { get; set; }
    }
}
