using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

        public SmtpEmailSender(ILogger<SmtpEmailSender> logger, IOptionsSnapshot<AppConfiguration> config)
        {
            _logger = logger;

            _config = config.Value.Mail;

            if (_config.Password == null)
                throw new ArgumentNullException("No mail password specified. Did you run 'dotnet user-secrets set \"AppSettings:Mail:Password\" \"<PASSWORD>\"' from the project root?");

            _mailer = new SmtpClient(_config.Host, _config.Port)
            {
                Credentials = new NetworkCredential(
                    _config.FromAddress,
                    _config.Password
                ),
                EnableSsl = true,
            };
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var message = new MailMessage(_config.FromAddress, email, subject, htmlMessage)
            {
                IsBodyHtml = true,
            };
            await _mailer.SendMailAsync(message).ConfigureAwait(false);
            _logger.LogInformation("mail send to {email}", email);
        }

        private bool _isDisplosed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisplosed) return;

            if (disposing)
            {
                _mailer.Dispose();
            }

            _isDisplosed = true;
        }
    }
}