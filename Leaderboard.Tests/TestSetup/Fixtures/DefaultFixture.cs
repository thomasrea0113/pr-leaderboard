using AutoFixture;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Leaderboards.Models;

namespace Leaderboard.Tests.TestSetup.Fixtures
{
    public class DefaultFixture : Fixture
    {
        public DefaultFixture()
        {
            this.Register(() => new Division
            {
                Name = $"Tag {this.Create<string>()}"
            });

            this.Register(() => new LeaderboardModel
            {
                Name = $"Leaderboard {this.Create<string>()}",
            });

            this.Register(() => new ApplicationUser
            {
                UserName = $"User_{this.Create<string>()}",
                Email = $"Email.{this.Create<string>()}@test.com"
            });
        }
    }
}