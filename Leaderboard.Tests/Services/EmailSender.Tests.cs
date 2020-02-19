using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Tests.TestSetup;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.Services
{
    // public static class ShellHelper
    // {
    //     public static string Bash(this string cmd)
    //     {
    //         var escapedArgs = cmd.Replace("\"", "\\\"");
            
    //         var process = new Process()
    //         {
    //             StartInfo = new ProcessStartInfo
    //             {
    //                 FileName = "/bin/bash",
    //                 Arguments = $"-c \"{escapedArgs}\"",
    //                 RedirectStandardOutput = true,
    //                 UseShellExecute = false,
    //                 CreateNoWindow = true,
    //             }
    //         };
    //         process.Start();
    //         process.WaitForExit();
    //         string result = process.StandardOutput.ReadToEnd().Trim();
    //         return result;
    //     }
    // }

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

            // TODO idealy we'd setup a postfix server, and use and send mail
            // locally to the root mailbox, but that bloats our docker container
            // for little benefit

            var to = $"thomasrea0113@gmail.com";
            await mailer.SendEmailAsync(to, "Testing Send", "Here's a test!");
        }
    }
}