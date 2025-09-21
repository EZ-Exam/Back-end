using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace teamseven.EzExam.Repository.Dtos;

public class QuestionDataResponse
{
    public int Id { get; set; }

    public string Content { get; set; }

    public string QuestionSource { get; set; }

    public string DifficultyLevel { get; set; }

    public int? LessonId { get; set; }

    public int? ChapterId { get; set; }

    public string? Image { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Frontend required properties from Question model
    public string? Formula { get; set; }
    public string? CorrectAnswer { get; set; }
    public string? Explanation { get; set; }
    public string Type { get; set; } = "multiple-choice";
    public List<string> Options { get; set; } = new List<string>();

    //related
    public string CreatedByUserName { get; set; }
    public string LessonName { get; set; }

    public string ChapterName { get; set; }
}
