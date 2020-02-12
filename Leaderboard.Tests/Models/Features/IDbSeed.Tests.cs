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
        });
    }
}