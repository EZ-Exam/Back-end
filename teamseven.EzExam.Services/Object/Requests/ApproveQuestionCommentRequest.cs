using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class ApproveQuestionCommentRequest
    {
        [Required(ErrorMessage = "Comment ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Comment ID must be a positive integer.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "IsApproved is required.")]
        public bool IsApproved { get; set; }
    }
}
