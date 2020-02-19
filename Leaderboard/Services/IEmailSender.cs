using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Leaderboard.Services
{
    public class SmtpEmailSenderConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string FromAddress { get; set; }
        public string Password { get; set; }

    }

    public class SmtpEmailSender : IEmailSender, IDisposable
    {
        private readonly ILogger<SmtpEmailSender> _logger;
        private readonly SmtpClient _mailer;
        private readonly SmtpEmailSenderConfig _config = new SmtpEmailSenderConfig();

        public SmtpEmailSender(ILogger<SmtpEmailSender> logger, IConfiguration config)
        {
            _logger = logger;

            config.Bind("Mail", _config);
            _mailer = new SmtpClient(_config.Host, _config.Port)
            {
                Credentials = new NetworkCredential(
                    _config.FromAddress,
                    _config.Password ?? throw new ArgumentNullException("No mail password specified. Did you run 'dotnet user-secrets set \"Mail:Password\" \"<PASSWORD>\"' from the project root?")
                ),
                EnableSsl = true,
            };
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await _mailer.SendMailAsync(new MailMessage(_config.FromAddress, email, subject, htmlMessage));
            _logger.LogInformation("mail send to {email}", email);
        }

        public void Dispose()
        {
            _mailer.Dispose();
        }
    }
}