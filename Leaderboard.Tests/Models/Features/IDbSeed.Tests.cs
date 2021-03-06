using System;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
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
        public async Task TestSeed()
        {
            using var _ = CreateScope(out var scope);

            await scope.GetRequiredService<IServiceProvider>().SeedDataAsync("development").ConfigureAwait(false);

            var ctx = scope.GetRequiredService<ApplicationDbContext>();
            var um = scope.GetRequiredService<AppUserManager>();

            var admin = await um.FindByNameAsync("Admin").ConfigureAwait(false);
            Assert.NotNull(admin);
            Assert.True(await um.IsInRoleAsync(admin, "admin").ConfigureAwait(false));
            Assert.Empty(admin.UserLeaderboards);

            var userName = "LifterDuder".Normalize().ToUpper();
            var user = await um.GetCompleteUserAsync(u => u.NormalizedUserName == userName).ConfigureAwait(false);

            Assert.NotNull(user);
            Assert.NotEmpty(user.UserLeaderboards);
            Assert.All(user.UserLeaderboards, lb => Assert.NotEmpty(lb.Leaderboard.Division.DivisionCategories));
            Assert.True(await um.IsInRoleAsync(user, "admin").ConfigureAwait(false));

            // TODO investigate... user.UserCategories is not empty, but the related properties on the object are not
            // lazy loaded (just return null instead of the proxy). We can get around this my querying the context
            // table directly
            Assert.NotEmpty(user.UserCategories);
            var ucs = ctx.UserCategories.AsQueryable()
                .Where(uc => uc.UserId == user.Id)
                .Select(uc => uc.Category.Name)
                .ToArray();

            Assert.Contains("Powerlifting", ucs);

            var ages = await ctx.Users.AsQueryable().Select(u => new
            {
                u.UserName,
                u.Age
            }).ToListAsync().ConfigureAwait(false);
        }
    }
}