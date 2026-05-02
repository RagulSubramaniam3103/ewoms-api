using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_ClassLibrary.EmailSending
{
    public class SendingEmail : IEmailService
    {
        public async  Task SendEmailAsync(string Toemail, string subject, string body)
        {
            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("ragulmani095@gmail.com", "jfqk aeql ahwz gnfg"),
                EnableSsl = true,
            };
            var message = new MailMessage
            {
                From = new MailAddress("ragulmani095@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(Toemail);
            await smtp.SendMailAsync(message);
        }
    }
}
