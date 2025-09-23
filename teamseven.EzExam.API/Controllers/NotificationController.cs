using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Services.OtherServices;

namespace teamseven.EzExam.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Produces("application/json")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(NotificationService notificationService, ILogger<NotificationController> logger)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("promotions")]
        [Authorize(Policy = "SaleStaffPolicy")]
        [SwaggerOperation(Summary = "Send promotion notification", Description = "Sends promotion notification to all users subscribed to 'promotion' topic.")]
        [SwaggerResponse(201, "Promotion notification sent successfully.", typeof(NotificationResponse))]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> SendPromotion([FromBody] NotificationRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Body))
            {
                _logger.LogWarning("Invalid model state for SendPromotion request.");
                return BadRequest(new { Message = "Title and body are required." });
            }

            try
            {
                var data = new Dictionary<string, string>
                {
                    { "type", "promotion" },
                    { "offer", request.Details ?? "No additional details" }
                };

                var messageId = await _notificationService.SendNotificationAsync(
                    title: request.Title,
                    body: request.Body,
                    data: data,
                    target: "promotion",
                    isTopic: true
                );

                _logger.LogInformation("Promotion notification sent successfully with message ID: {MessageId}", messageId);
                return StatusCode(201, new NotificationResponse
                {
                    Message = "Promotion notification sent successfully.",
                    MessageId = messageId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending promotion notification.");
                return StatusCode(500, new { Message = "An error occurred while sending the promotion notification." });
            }
        }

        [HttpPost("announcements")]
        [Authorize(Policy = "SaleStaffPolicy")]
        [SwaggerOperation(Summary = "Send announcement notification", Description = "Sends announcement notification to all users subscribed to 'announcements' topic.")]
        [SwaggerResponse(201, "Announcement notification sent successfully.", typeof(NotificationResponse))]
        [SwaggerResponse(400, "Invalid request data.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> SendAnnouncement([FromBody] NotificationRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Body))
            {
                _logger.LogWarning("Invalid model state for SendAnnouncement request.");
                return BadRequest(new { Message = "Title and body are required." });
            }

            try
            {
                var data = new Dictionary<string, string>
                {
                    { "type", "announcement" },
                    { "details", request.Details ?? "No additional details" }
                };

                var messageId = await _notificationService.SendNotificationAsync(
                    title: request.Title,
                    body: request.Body,
                    data: data,
                    target: "announcements",
                    isTopic: true
                );

                _logger.LogInformation("Announcement notification sent successfully with message ID: {MessageId}", messageId);
                return StatusCode(201, new NotificationResponse
                {
                    Message = "Announcement notification sent successfully.",
                    MessageId = messageId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending announcement notification.");
                return StatusCode(500, new { Message = "An error occurred while sending the announcement notification." });
            }
        }
    }

    public class NotificationRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? Details { get; set; }
    }

    public class NotificationResponse
    {
        public string Message { get; set; } = string.Empty;
        public string MessageId { get; set; } = string.Empty;
    }
}