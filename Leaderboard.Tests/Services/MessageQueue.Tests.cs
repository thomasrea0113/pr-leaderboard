using System.Linq;
using System.Net.Http;
using Leaderboard.Services;
using Leaderboard.Tests.TestSetup;
using Leaderboard.Tests.TestSetup.Fixtures;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.Services
{
    public class MessageQueueTests : BaseTestClass
    {
        public MessageQueueTests(WebOverrideFactory factory) : base(factory)
        {
        }

        [Fact]
        public void TestEnqueue()
        {
            using var _ = CreateScope(out var scope);
            // using (var _ = CreateScope(out var scope))
            // {
                var queue = scope.GetRequiredService<IMessageQueue>();

                Assert.Empty(queue.GetAllMessages());

                queue.PushMessage("here we go!");

                Assert.Equal("here we go!", queue.GetAllMessages(true).Single());
                Assert.Equal("here we go!", queue.GetAllMessages().Single());

                // TempData only gets cleared after the HttpContext processes a successful
                // request (status code 200-299). So the read doesn't clear the messages.
                // TODO implement test to actually clear the TempData
                Assert.Equal("here we go!", queue.GetAllMessages().Single());

            queue.PushMessage("here we go 1!");
            queue.PushMessage("here we go 2!");
            queue.PushMessage("here we go 3!");

            Assert.Equal(4, queue.GetAllMessages(true).Count());
            Assert.Equal(4, queue.GetAllMessages().Count());
        }
    }
}