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

            var admin = await um.FindByNameAsync("Admin");
            Assert.NotNull(admin);
            Assert.True(await um.IsInRoleAsync(admin, "Admin"));
            Assert.Empty(admin.UserLeaderboards);

            var user = await um.FindByNameAsync("LifterDuder");
            Assert.NotNull(user);
            Assert.NotEmpty(user.UserLeaderboards);
            Assert.All(user.UserLeaderboards, lb => {
                Assert.Single(lb.Leaderboard.Division.DivisionCategories);
                Assert.Equal("Powerlifting", lb.Leaderboard.Division.DivisionCategories.First().Category.Name);
            });

            Assert.Single(user.Interests);
            Assert.Equal("Powerlifting", user.Interests.Single().Category.Name);

            var ages = await ctx.Users.AsQueryable().Select(u => new {
                u.UserName,
                u.Age
            }).ToListAsync();
        });
    }
}