using System.ComponentModel.DataAnnotations;

namespace teamseven.EzExam.Repository.Dtos
{
    public class SolutionReportRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Solution ID must be a positive number.")]
        public int SolutionId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ReportedByUserId must be a positive number.")]
        public int ReportedByUserId { get; set; }

        [Required(ErrorMessage = "Reason is required.")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
        public string Reason { get; set; }
    }
}
