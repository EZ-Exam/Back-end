using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class UpdateQuestionCommentRequest
    {
        [Required(ErrorMessage = "Comment ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Comment ID must be a positive integer.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [StringLength(2000, ErrorMessage = "Content cannot exceed 2000 characters.")]
        public string Content { get; set; } = string.Empty;

        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        public int? Rating { get; set; }

        public bool? IsHelpful { get; set; }
    }
}
