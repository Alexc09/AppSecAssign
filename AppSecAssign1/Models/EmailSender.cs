using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppSecAssign1.Models
{
    public interface IMailService
    {
        Task SendEmailAsync(string emailAddress, string name, string content);
    }

    public class EmailSender: IMailService
    {
        private readonly IConfiguration _config;
        private static string ApiKey;

        public EmailSender(IConfiguration configuration)
        {
            _config = configuration;
            ApiKey = _config["SendGridAPIKey"];
        }

        public async Task SendEmailAsync(string emailAddress, string name, string content)
        {
            Console.WriteLine(ApiKey);
            var client = new SendGridClient(ApiKey);
            var from = new EmailAddress("203273a@mymail.nyp.edu.sg", "ConnectSIT");
            var subject = "Your OTP";
            var to = new EmailAddress(emailAddress, name);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
