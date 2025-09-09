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
        /// G?i email d?n d?a ch? du?c ch? d?nh v?i ti�u d? v� n?i dung c? th?.
        /// </summary>
        /// <param name="toEmail">�?a ch? email ngu?i nh?n.</param>
        /// <param name="subject">Ti�u d? email.</param>
        /// <param name="body">N?i dung email.</param>
        void SendEmail(string toEmail, string subject, string body);
    }
}
