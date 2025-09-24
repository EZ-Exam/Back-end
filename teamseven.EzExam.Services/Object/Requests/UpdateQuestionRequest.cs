using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class UpdateQuestionRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Difficulty level ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Difficulty level ID must be a positive integer.")]
        public int DifficultyLevelId { get; set; }

        [StringLength(500, ErrorMessage = "Question source cannot exceed 500 characters.")]
        public string? QuestionSource { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Subject ID must be a positive integer.")]
        public int? SubjectId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Chapter ID must be a positive integer.")]
        public int? ChapterId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Lesson ID must be a positive integer.")]
        public int? LessonId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Textbook ID must be a positive integer.")]
        public int? TextbookId { get; set; }

        public string? Image { get; set; }

        [StringLength(20, ErrorMessage = "Question type cannot exceed 20 characters.")]
        public string? QuestionType { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Template question ID must be a positive integer.")]
        public int? TemplateQuestionId { get; set; }

        [StringLength(5000, ErrorMessage = "Formula cannot exceed 5000 characters.")]
        public string? Formula { get; set; }

        [StringLength(5000, ErrorMessage = "Correct answer cannot exceed 5000 characters.")]
        public string? CorrectAnswer { get; set; }

        [StringLength(5000, ErrorMessage = "Explanation cannot exceed 5000 characters.")]
        public string? Explanation { get; set; }

        public List<string>? Options { get; set; }
    }
}
