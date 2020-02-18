using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Tests.TestSetup;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.Services
{
    public static class ShellHelper
    {
        public static string Bash(this string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");
            
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            process.WaitForExit();
            string result = process.StandardOutput.ReadToEnd().Trim();
            return result;
        }
    }

    public class EmailSenderTests : BaseTestClass
    {
        public EmailSenderTests(WebOverrideFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task TestSendAsync()
        {
            using var _ = CreateScope(out var scope);
            var mailer = scope.GetRequiredService<IEmailSender>();

            var me = "whoami".Bash();
            var to = $"{me}@localhost";

            $"rm /var/mail/{me}".Bash();
            await mailer.SendEmailAsync(to, "Testing Send", "Here's a test!");
            await mailer.SendEmailAsync(to, "Testing Send", "Here's a test 2!");
            await mailer.SendEmailAsync(to, "Testing Send", "Here's a test 3!");

            var mail = $"cat /var/mail/{me}".Bash();

            // should have 3 messages waiting for us
            Assert.Equal(3, mail.Split('\n').Where(l => l.StartsWith("Message-Id:")).Count());
            
            // clear the messages
            $"rm /var/mail/{me}".Bash();
        }
    }
}