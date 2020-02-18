using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Threading;
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
        public string FromName { get; set; }

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
            _mailer = new SmtpClient(_config.Host, _config.Port);
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