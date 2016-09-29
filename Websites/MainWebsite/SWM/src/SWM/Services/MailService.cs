using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace SWM.Services
{
    public class MailService : IMailService
    {
        private IConfigurationRoot _config;
        public MailService(IConfigurationRoot config)
        {
            _config = config;
        }
        public int SendMail(string fromName, string from, string toName, string to, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, from));
                message.To.Add(new MailboxAddress(toName, to));
                message.Subject = subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = body;

                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect("smtp.sendgrid.net", 587, false);
                    client.Authenticate(_config["smtp:user"], _config["smtp:pwd"]);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Send(message);
                    client.Disconnect(true);
                    return 1;
                }
            }
            catch(Exception)
            {
                return 0;
            }
        }
    }
}
