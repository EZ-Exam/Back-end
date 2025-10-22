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
        public string Mode { get; set; } = string.Empty; // "review" hoặc "advanced"

        [Range(1, 5, ErrorMessage = "HistoryCount must be between 1 and 5")]
        public int HistoryCount { get; set; } = 5; // Số lịch sử làm bài gần nhất

        // Optional filters - User chọn hoặc auto-detect từ lịch sử
        // Nếu user chỉ định → AI BẮT BUỘC phải tuân theo
        // Nếu user không chỉ định → Auto-detect từ lịch sử
        
        /// <summary>
        /// Danh sách ID môn học (có thể chọn nhiều). Ví dụ: [1, 2, 3] để gen đề từ Toán, Vật lý, Hóa.
        /// Nếu null hoặc rỗng → Auto-detect từ lịch sử thi.
        /// </summary>
        public List<int>? SubjectIds { get; set; }
        
        /// <summary>
        /// Danh sách ID khối lớp (có thể chọn nhiều). Ví dụ: [10, 11, 12] để gen đề từ lớp 10-12.
        /// Nếu null hoặc rỗng → Auto-detect từ lịch sử thi.
        /// </summary>
        public List<int>? GradeIds { get; set; }
        
        /// <summary>
        /// Danh sách ID chương (có thể chọn nhiều). Ví dụ: [1, 2, 3] để gen đề từ các chương cụ thể.
        /// Nếu null hoặc rỗng → Lấy từ tất cả chương trong môn học.
        /// </summary>
        public List<int>? ChapterIds { get; set; }
        
        /// <summary>
        /// Danh sách ID bài học (có thể chọn nhiều). Ví dụ: [1, 5, 10] để gen đề từ các bài cụ thể.
        /// Nếu null hoặc rỗng → Lấy từ tất cả bài học trong chương/môn.
        /// </summary>
        public List<int>? LessonIds { get; set; }
        
        /// <summary>
        /// ID độ khó (1=EASY, 2=MEDIUM, 3=HARD). Nếu null → Lấy tất cả độ khó.
        /// </summary>
        public int? DifficultyLevelId { get; set; }
    }
}
