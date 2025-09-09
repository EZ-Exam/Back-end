using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Repository.Dtos;

public class QuestionDataRequest
{

    [Required(ErrorMessage = "Content is required.")]
    [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters.")]
    public string Content { get; set; }

    [StringLength(500, ErrorMessage = "Question source cannot exceed 500 characters.")]
    public string QuestionSource { get; set; }

    [Required(ErrorMessage = "Difficulty level ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Difficulty level ID must be a positive integer.")]
    public int DifficultyLevelId { get; set; }

    [Required(ErrorMessage = "Lesson ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Lesson ID must be a positive integer.")]
    public int? LessonId { get; set; }
    public string? Image { get; set; }

    [Required(ErrorMessage = "Created by user ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Created by user ID must be a positive integer.")]
    public int CreatedByUserId { get; set; }
}
