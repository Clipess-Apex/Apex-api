using System.Net;
using System.Net.Mail;

namespace Clipess.API.Properties.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpServer = "smtp.gmail.com";
            var smtpPort = 587;
            var senderEmail = "prabhashpramodha99@gmail.com";
            var senderPassword = "wafg dqor spwg pwwz";
            var useSsl = true;

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.EnableSsl = useSsl;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);

                var mailMessage = new MailMessage(senderEmail, toEmail, subject, body);
                mailMessage.IsBodyHtml = true;

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
