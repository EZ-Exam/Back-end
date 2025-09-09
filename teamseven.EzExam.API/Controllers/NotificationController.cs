using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly NotificationService _notificationService;

    public NotificationController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// G?i thông báo qu?ng cáo t?i t?t c? ngu?i dùng dã subscribe topic "promotion".
    /// </summary>
    /// <param name="request">Thông tin thông báo qu?ng cáo.</param>
    /// <returns>K?t qu? g?i thông báo.</returns>
    /// <response code="201">Thông báo qu?ng cáo du?c g?i thành công.</response>
    /// <response code="400">Tiêu d? ho?c n?i dung tr?ng.</response>
    /// <response code="500">L?i server khi g?i thông báo.</response>
    [Authorize(Policy = "SaleStaffPolicy")]
    [HttpPost("promotions")]
    [SwaggerOperation(Summary = "G?i thông báo qu?ng cáo", Description = "G?i thông báo qu?ng cáo t?i topic 'promotion'.")]
    [ProducesResponseType(typeof(NotificationResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> SendPromotion([FromBody] NotificationRequest request)
    {
        if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Body))
        {
            return BadRequest("Tiêu d? và n?i dung không du?c d? tr?ng.");
        }

        try
        {
            var data = new Dictionary<string, string>
            {
                { "type", "promotion" },
                { "offer", request.Details ?? "Không có chi ti?t uu dãi" }
            };

            var messageId = await _notificationService.SendNotificationAsync(
                title: request.Title,
                body: request.Body,
                data: data,
                target: "promotion",
                isTopic: true
            );

            return StatusCode(201, new NotificationResponse
            {
                Message = "Thông báo qu?ng cáo dã g?i.",
                MessageId = messageId
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"L?i khi g?i thông báo qu?ng cáo: {ex.Message}");
        }
    }

    /// <summary>
    /// G?i thông báo chung t?i t?t c? ngu?i dùng dã subscribe topic "announcements".
    /// </summary>
    /// <param name="request">Thông tin thông báo chung.</param>
    /// <returns>K?t qu? g?i thông báo.</returns>
    /// <response code="201">Thông báo chung du?c g?i thành công.</response>
    /// <response code="400">Tiêu d? ho?c n?i dung tr?ng.</response>
    /// <response code="500">L?i server khi g?i thông báo.</response>
    [HttpPost("announcements")]
    [Authorize(Policy = "SaleStaffPolicy")]
    [SwaggerOperation(Summary = "G?i thông báo chung", Description = "G?i thông báo chung t?i topic 'announcements'.")]
    [ProducesResponseType(typeof(NotificationResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> SendAnnouncement([FromBody] NotificationRequest request)
    {
        if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Body))
        {
            return BadRequest("Tiêu d? và n?i dung không du?c d? tr?ng.");
        }

        try
        {
            var data = new Dictionary<string, string>
            {
                { "type", "announcement" },
                { "details", request.Details ?? "Không có chi ti?t thêm" }
            };

            var messageId = await _notificationService.SendNotificationAsync(
                title: request.Title,
                body: request.Body,
                data: data,
                target: "announcements",
                isTopic: true
            );

            return StatusCode(201, new NotificationResponse
            {
                Message = "Thông báo chung dã g?i.",
                MessageId = messageId
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"L?i khi g?i thông báo chung: {ex.Message}");
        }
    }
}

public class NotificationRequest
{
    public string Title { get; set; }
    public string Body { get; set; }
    public string Details { get; set; }
}

public class NotificationResponse
{
    public string Message { get; set; }
    public string MessageId { get; set; }
}
