using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController] // Ch? ra r?ng d�y l� m?t API Controller
    //[Route("api/status")] // �?nh nghia route co b?n cho Controller n�y. V� d?: /api/testserver
    public class ServerStatusController : ControllerBase // K? th?a t? ControllerBase cho c�c API Controller
    {
        private readonly ILogger<ServerStatusController> _logger;

        // Constructor (t�y ch?n: d�ng d? inject logger)
        public ServerStatusController(ILogger<ServerStatusController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// H�m ki?m tra tr?ng th�i server.
        /// </summary>
        /// <returns>Tr? v? m?t th�ng b�o JSON "Server is running!".</returns>
        [HttpGet("status")] // �?nh nghia d�y l� m?t HTTP GET request, route l� /api/testserver/status
        public IActionResult GetServerStatus()
        {
            _logger.LogInformation("GET request to /api/testserver/status received.");
            return Ok(new { message = "Server is running!", timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// H�m tr? v? th�ng b�o t�y ch?nh d?a tr�n d?u v�o.
        /// </summary>
        /// <param name="name">T�n b?n mu?n g?i l?i ch�o.</param>
        /// <returns>Tr? v? m?t th�ng b�o ch�o m?ng.</returns>
        [HttpGet("status/{name}")] // �?nh nghia HTTP GET request v?i tham s? trong route: /api/testserver/hello/John
        public IActionResult SayHello(string name)
        {
            _logger.LogInformation($"GET request to /api/testserver/hello/{name} received.");
            return Ok($"Hello, {name}! Your backend is working.");
        }

    }
}
