using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController] // Ch? ra r?ng dây là m?t API Controller
    //[Route("api/status")] // Ð?nh nghia route co b?n cho Controller này. Ví d?: /api/testserver
    public class ServerStatusController : ControllerBase // K? th?a t? ControllerBase cho các API Controller
    {
        private readonly ILogger<ServerStatusController> _logger;

        // Constructor (tùy ch?n: dùng d? inject logger)
        public ServerStatusController(ILogger<ServerStatusController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Hàm ki?m tra tr?ng thái server.
        /// </summary>
        /// <returns>Tr? v? m?t thông báo JSON "Server is running!".</returns>
        [HttpGet("status")] // Ð?nh nghia dây là m?t HTTP GET request, route là /api/testserver/status
        public IActionResult GetServerStatus()
        {
            _logger.LogInformation("GET request to /api/testserver/status received.");
            return Ok(new { message = "Server is running!", timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Hàm tr? v? thông báo tùy ch?nh d?a trên d?u vào.
        /// </summary>
        /// <param name="name">Tên b?n mu?n g?i l?i chào.</param>
        /// <returns>Tr? v? m?t thông báo chào m?ng.</returns>
        [HttpGet("status/{name}")] // Ð?nh nghia HTTP GET request v?i tham s? trong route: /api/testserver/hello/John
        public IActionResult SayHello(string name)
        {
            _logger.LogInformation($"GET request to /api/testserver/hello/{name} received.");
            return Ok($"Hello, {name}! Your backend is working.");
        }

    }
}
