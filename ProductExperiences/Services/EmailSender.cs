using Microsoft.Extensions.Options;
using ProductExperiences.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ProductExperiences.Services
{
    public class EmailSender: IEmailSender
    {
        private EmailSettings _emailSettings;

        public EmailSender(/*IOptions<EmailSettings> emailSettings*/)
        {
            _emailSettings = new EmailSettings()
            {
                MailServer = "smtp.gmail.com",
                MailPort = 587,
                SenderName = "Product experiences",
                Sender = "pexperiences100@gmail.com",
                Password = "pexp5060"
            };
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {      

            try
            {
                // Credentials
                var credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password);

                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.Sender, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mail.To.Add(new MailAddress(email));

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = _emailSettings.MailPort,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = _emailSettings.MailServer,
                    EnableSsl = true,
                    Credentials = credentials
                };

                // Send it...         
                client.Send(mail);
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }

            return Task.CompletedTask;
        }
    }
}
