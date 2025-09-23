using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class StartTestSessionRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer.")]
        public int UserId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Exam ID must be a positive integer.")]
        public int ExamId { get; set; }

        [Range(0, 100, ErrorMessage = "Passing score must be between 0 and 100.")]
        public decimal? PassingScore { get; set; }

        [StringLength(500, ErrorMessage = "Device info cannot exceed 500 characters.")]
        public string? DeviceInfo { get; set; }
    }
}
