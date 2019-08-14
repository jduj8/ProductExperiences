using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit;

namespace ProductExperiences.Helpers
{
    public class MessageHelper
    {
        public static async Task  SendEmailFromAppToUserAsync(string email, string subject, string content)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Product Experiences", "pexperiences100@gmail.com"));
            message.To.Add(new MailboxAddress(email));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = content
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("pexperiences100@gmail.com", "pexp5060");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
