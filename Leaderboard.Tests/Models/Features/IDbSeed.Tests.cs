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
        /// </summary>
        /// <param name="executionCount"></param>
        /// <returns></returns>
        [Fact]
        public async Task TestSeed() => await WithScopeAsync(async scope =>
        {
            var ctx = scope.GetRequiredService<ApplicationDbContext>();
            var um = scope.GetRequiredService<AppUserManager>();

            var admin = await um.FindByNameAsync("admin-user-id");

            Assert.NotNull(admin);

            Assert.True(await um.IsInRoleAsync(admin, "Admin"));

            var boards = await ctx.Set<LeaderboardModel>().AsQueryable()
                .Where(b => b.Name == "Bench 1 Rep Max" || b.Name == "Deadlift 1 Rep Max")
                .ToListAsync();

            Assert.Equal(2, admin.UserLeaderboards.Count);
            Assert.All(admin.UserLeaderboards.Select(ub => ub.Leaderboard), b=> boards.Contains(b));
        });

        /// <summary>
        /// Tests the data seeding method (both for identity and the calls to HasData on the model builder).
        /// </summary>
        /// <param name="executionCount"></param>
        /// <returns></returns>
        [Fact]
        public async Task TestDivisionSeed() => await WithScopeAsync(async scope =>
        {
            var ctx = scope.GetRequiredService<ApplicationDbContext>();

            // these numbers will change whever the seed files change
            Assert.Equal(3, await EntityFrameworkQueryableExtensions.CountAsync(ctx.Divisions));
            Assert.Equal(3, await EntityFrameworkQueryableExtensions.CountAsync(ctx.WeightClasses));
            Assert.All(ctx.Divisions, d => Assert.Single(d.WeightClasses));
            Assert.All(ctx.WeightClasses, wc => Assert.Single(wc.Divisions));
        });
    }
}