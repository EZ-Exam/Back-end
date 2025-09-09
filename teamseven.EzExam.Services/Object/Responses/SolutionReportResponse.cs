using System;

namespace teamseven.EzExam.Repository.Dtos
{
    public class SolutionReportResponse
    {
        public int Id { get; set; }
        public int SolutionId { get; set; }
        public int ReportedByUserId { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime ReportDate { get; set; }

        // Related info
        public string ReporterEmail { get; set; }
        public string ReporterFullName { get; set; }
    }
}
