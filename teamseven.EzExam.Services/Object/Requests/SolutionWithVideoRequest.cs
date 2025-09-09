using Microsoft.AspNetCore.Http;

public class SolutionWithVideoRequest
{
    public int? QuestionId { get; set; }
    public string Content { get; set; }

    public IFormFile VideoFile { get; set; } // form-data
}
