using System.Threading.Tasks;
using Leaderboard.Tests.TestSetup;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.Services
{
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

            // TODO idealy we'd setup a postfix server, and use send mail
            // locally to the root mailbox, but that bloats our docker container
            // for little benefit

            // send the email. If no exception is throw, then all is well
            var to = $"thomasrea0113@gmail.com";
            await mailer.SendEmailAsync(to, "PR Leaderboard Unit Test", "This email was generated as part of the Leaderboard Unit Tests. You can ignore this message.")
                .ConfigureAwait(false);
        }
    }
}