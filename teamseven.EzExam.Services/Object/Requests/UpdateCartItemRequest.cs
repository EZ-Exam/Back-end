using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class UpdateCartItemRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer.")]
        public int UserId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Question ID must be a positive integer.")]
        public int QuestionId { get; set; }

        public bool IsSelected { get; set; }

        [StringLength(1000, ErrorMessage = "User notes cannot exceed 1000 characters.")]
        public string? UserNotes { get; set; }

        [StringLength(20, ErrorMessage = "Difficulty preference cannot exceed 20 characters.")]
        public string? DifficultyPreference { get; set; }
    }
}
