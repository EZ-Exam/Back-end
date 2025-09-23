using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class NotificationService
{
    /// <summary>
    /// G?i notification qua Firebase.
    /// </summary>
    /// <param name="title">Ti�u d? noti.</param>
    /// <param name="body">N?i dung noti.</param>
    /// <param name="data">D? li?u t�y ch?nh (v� d?: transactionId cho x�c minh).</param>
    /// <param name="target">Token device (cho c� nh�n) ho?c topic (cho qu?ng c�o, v� d? "promotion").</param>
    /// <param name="isTopic">True n?u target l� topic (qu?ng c�o), false n?u l� token c� nh�n.</param>
    public async Task<string> SendNotificationAsync(string title, string body, Dictionary<string, string> data = null, string target = null, bool isTopic = false)
    {
        if (string.IsNullOrEmpty(target))
        {
            throw new ArgumentException("Target (token ho?c topic) kh�ng du?c d? tr?ng.");
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
            message.Topic = target; // V� d?: "promotion" cho qu?ng c�o
        }
        else
        {
            message.Token = target; // Token device cho x�c minh giao d?ch
        }

        try
        {
            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            return response; // Tr? v? message ID n?u th�nh c�ng
        }
        catch (Exception ex)
        {
            Console.WriteLine($"L?i g?i noti: {ex.Message}");
            throw;
        }
    }
}
