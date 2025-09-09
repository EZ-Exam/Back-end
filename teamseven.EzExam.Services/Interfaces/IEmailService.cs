using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// G?i email d?n d?a ch? du?c ch? d?nh v?i tiêu d? và n?i dung c? th?.
        /// </summary>
        /// <param name="toEmail">Ð?a ch? email ngu?i nh?n.</param>
        /// <param name="subject">Tiêu d? email.</param>
        /// <param name="body">N?i dung email.</param>
        void SendEmail(string toEmail, string subject, string body);
    }
}
