using Mailjet.Client;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mailjet.Client.Resources;

namespace OmegasysWeb.Utilidades
{
    public class EmailSender : IEmailSender
    {
        private readonly MailjetClient _mailjetClient;

        public EmailSender(IConfiguration configuration)
        {
            var apiKey = configuration.GetValue<string>("MailJet:ApiKey");
            var secretKey = configuration.GetValue<string>("MailJet:SecretKey");
            _mailjetClient = new MailjetClient(apiKey, secretKey);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
            .Property(Send.FromEmail, "nestorperezbest@gmail.com")
            .Property(Send.FromName, "Omega Shop")
            .Property(Send.Subject, subject)
            .Property(Send.TextPart, "")
            .Property(Send.HtmlPart, htmlMessage)
            .Property(Send.Recipients, new JArray {
            new JObject {
                 {"Email", email}
            }
            });

            var response = await _mailjetClient.PostAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Email sent successfully to {email}");
                return;
            }
            else
            {
                Console.WriteLine($"Failed to send email to {email}. Status code: {response.StatusCode}");
                throw new Exception($"Failed to send email: {response.StatusCode} - {response.Content}");
            }
        }
    }
}