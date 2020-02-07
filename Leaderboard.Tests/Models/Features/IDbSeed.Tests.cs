using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Data;
using Leaderboard.Tests.TestSetup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.Models.Features
{
    public class IDbSeedTests : BaseTestClass
    {
        public IDbSeedTests(WebOverrideFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Tests the data seeding method (both for identity and the calls to HasData on the model builder).
        /// This test is executed twice. The first time the test will create all the models (assuming a fresh
        /// database), and the second time the seed method should retrieve the models
        /// </summary>
        /// <param name="executionCount"></param>
        /// <returns></returns>
        [Fact]
        public async Task TestSeed() => await WithScopeAsync(async scope =>
        {
            var ctx = scope.GetRequiredService<ApplicationDbContext>();
            var um = scope.GetRequiredService<AppUserManager>();

            var admin = await um.FindByNameAsync("SooperDooperLifter");

            Assert.NotNull(admin);

            Assert.True(await um.IsInRoleAsync(admin, "Admin"));

            var boards = await ctx.Set<LeaderboardModel>().AsQueryable()
                .Where(b => b.Name == "Bench 1 Rep Max" || b.Name == "Deadlift 1 Rep Max")
                .ToListAsync();

            Assert.Equal(2, admin.UserLeaderboards.Count);
            Assert.All(admin.UserLeaderboards.Select(ub => ub.Leaderboard), b=> boards.Contains(b));
        });
    }
}