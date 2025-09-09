using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class NotificationService
{
    /// <summary>
    /// G?i notification qua Firebase.
    /// </summary>
    /// <param name="title">Tiêu d? noti.</param>
    /// <param name="body">N?i dung noti.</param>
    /// <param name="data">D? li?u tùy ch?nh (ví d?: transactionId cho xác minh).</param>
    /// <param name="target">Token device (cho cá nhân) ho?c topic (cho qu?ng cáo, ví d? "promotion").</param>
    /// <param name="isTopic">True n?u target là topic (qu?ng cáo), false n?u là token cá nhân.</param>
    public async Task<string> SendNotificationAsync(string title, string body, Dictionary<string, string> data = null, string target = null, bool isTopic = false)
    {
        if (string.IsNullOrEmpty(target))
        {
            throw new ArgumentException("Target (token ho?c topic) không du?c d? tr?ng.");
        }

        var message = new Message()
        {
            Notification = new Notification()
            {
                Title = title,
                Body = body
            },
            Data = data ?? new Dictionary<string, string>(),
        };

        if (isTopic)
        {
            message.Topic = target; // Ví d?: "promotion" cho qu?ng cáo
        }
        else
        {
            message.Token = target; // Token device cho xác minh giao d?ch
        }

        try
        {
            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            return response; // Tr? v? message ID n?u thành công
        }
        catch (Exception ex)
        {
            Console.WriteLine($"L?i g?i noti: {ex.Message}");
            throw;
        }
    }
}
