using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Services.Object.Requests
{
    public class GenerateAIExamRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "UserId must be a positive integer")]
        public int UserId { get; set; }

        [Required]
        [Range(1, 50, ErrorMessage = "QuestionCount must be between 1 and 50")]
        public int QuestionCount { get; set; }

        [Required]
        [RegularExpression("^(review|advanced)$", ErrorMessage = "Mode must be either 'review' or 'advanced'")]
        public string Mode { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "HistoryCount must be between 1 and 5")]
        public int HistoryCount { get; set; } = 5;

        
        /// <summary>
        /// </summary>
        public List<int>? SubjectIds { get; set; }
        
        /// <summary>
        /// </summary>
        public List<int>? GradeIds { get; set; }
        
        /// <summary>
        /// </summary>
        public List<int>? ChapterIds { get; set; }
        
        /// <summary>
        /// </summary>
        public List<int>? LessonIds { get; set; }
        
        /// <summary>
        /// </summary>
        public int? DifficultyLevelId { get; set; }
    }
}
