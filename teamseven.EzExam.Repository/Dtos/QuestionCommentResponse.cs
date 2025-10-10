using System;

namespace teamseven.EzExam.Repository.Dtos
{
    public class QuestionCommentResponse
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int? ParentCommentId { get; set; }
        public int Rating { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsHelpful { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // User information
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;

        // Reply count for main comments
        public int ReplyCount { get; set; } = 0;

        // Replies for main comments (flat structure)
        public List<QuestionCommentResponse> Replies { get; set; } = new List<QuestionCommentResponse>();
    }
}
