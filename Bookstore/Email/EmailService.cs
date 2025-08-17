using Bookstore.DTO.Email;
using Bookstore.Email;
using MimeKit;
using System.ComponentModel;
using System.Net.Mail;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Bookstore.Email
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {

            this._config = config;

        }
        public async Task SendEmail(string email, string code, string component, string subject, string message)
        {
            var result = new EmailDto(email, "ahmedyasserr552@gmail.com", subject, EmailStringBody.SendActiveEmail(email, code, component, message));
            await SendEmailService(result);
        }

        public async Task SendEmailResetPassword(string email, string code, string component, string subject, string message)
        {
            var result = new EmailDto(email, "ahmedyasserr552@gmail.com", subject, EmailStringBody.SendForgotPassword(email, code, component, message));
            await SendEmailService(result);
        }
        public async Task SendEmailService(EmailDto emailDTO)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(_config["EmailSetting:UserName"], _config["EmailSetting:UserName"]));
            message.Subject = emailDTO.subject;
            message.To.Add(new MailboxAddress(emailDTO.To, emailDTO.To));
            message.Body = new TextPart("html")
            {
                Text = emailDTO.content
            };
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    await smtp.ConnectAsync(_config["EmailSetting:Host"],
                        int.Parse(_config["EmailSetting:Port"]), true);
                    await smtp.AuthenticateAsync(_config["EmailSetting:UserName"],
                        _config["EmailSetting:Password"]);
                    await smtp.SendAsync(message);
                }
                catch { }
                finally
                {
                    smtp.Disconnect(true);
                    smtp.Dispose();
                }
            }
        }
    }
}
