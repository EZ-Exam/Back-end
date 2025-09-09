using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Requests
{
    public class CreateQuestionRequest
    {
        [Required(ErrorMessage = "Content is required")]
        [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Question source is required")]
        [StringLength(500, ErrorMessage = "Question source cannot exceed 500 characters")]
        public string QuestionSource { get; set; }

        [Required(ErrorMessage = "Difficulty level ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Difficulty level ID must be a positive integer")]
        public int DifficultyLevelId { get; set; }

        [Required(ErrorMessage = "Lesson ID is required")]
        public int? LessonId { get; set; }
        public string? Image { get; set; }

        [Required(ErrorMessage = "Created by user ID is required")]
        public int CreatedByUserId { get; set; }
    }
}
