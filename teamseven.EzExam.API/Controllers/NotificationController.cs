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
    /// G?i th�ng b�o qu?ng c�o t?i t?t c? ngu?i d�ng d� subscribe topic "promotion".
    /// </summary>
    /// <param name="request">Th�ng tin th�ng b�o qu?ng c�o.</param>
    /// <returns>K?t qu? g?i th�ng b�o.</returns>
    /// <response code="201">Th�ng b�o qu?ng c�o du?c g?i th�nh c�ng.</response>
    /// <response code="400">Ti�u d? ho?c n?i dung tr?ng.</response>
    /// <response code="500">L?i server khi g?i th�ng b�o.</response>
    [Authorize(Policy = "SaleStaffPolicy")]
    [HttpPost("promotions")]
    [SwaggerOperation(Summary = "G?i th�ng b�o qu?ng c�o", Description = "G?i th�ng b�o qu?ng c�o t?i topic 'promotion'.")]
    [ProducesResponseType(typeof(NotificationResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> SendPromotion([FromBody] NotificationRequest request)
    {
        if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Body))
        {
            return BadRequest("Ti�u d? v� n?i dung kh�ng du?c d? tr?ng.");
        }

        try
        {
            var data = new Dictionary<string, string>
            {
                { "type", "promotion" },
                { "offer", request.Details ?? "Kh�ng c� chi ti?t uu d�i" }
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
                Message = "Th�ng b�o qu?ng c�o d� g?i.",
                MessageId = messageId
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"L?i khi g?i th�ng b�o qu?ng c�o: {ex.Message}");
        }
    }

    /// <summary>
    /// G?i th�ng b�o chung t?i t?t c? ngu?i d�ng d� subscribe topic "announcements".
    /// </summary>
    /// <param name="request">Th�ng tin th�ng b�o chung.</param>
    /// <returns>K?t qu? g?i th�ng b�o.</returns>
    /// <response code="201">Th�ng b�o chung du?c g?i th�nh c�ng.</response>
    /// <response code="400">Ti�u d? ho?c n?i dung tr?ng.</response>
    /// <response code="500">L?i server khi g?i th�ng b�o.</response>
    [HttpPost("announcements")]
    [Authorize(Policy = "SaleStaffPolicy")]
    [SwaggerOperation(Summary = "G?i th�ng b�o chung", Description = "G?i th�ng b�o chung t?i topic 'announcements'.")]
    [ProducesResponseType(typeof(NotificationResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> SendAnnouncement([FromBody] NotificationRequest request)
    {
        if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Body))
        {
            return BadRequest("Ti�u d? v� n?i dung kh�ng du?c d? tr?ng.");
        }

        try
        {
            var data = new Dictionary<string, string>
            {
                { "type", "announcement" },
                { "details", request.Details ?? "Kh�ng c� chi ti?t th�m" }
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
                Message = "Th�ng b�o chung d� g?i.",
                MessageId = messageId
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"L?i khi g?i th�ng b�o chung: {ex.Message}");
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
