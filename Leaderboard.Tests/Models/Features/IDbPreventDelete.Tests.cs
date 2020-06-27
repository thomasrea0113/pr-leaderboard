using System.Threading.Tasks;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Data;
using Leaderboard.Models.Features;
using Leaderboard.Tests.TestSetup;
using Leaderboard.Tests.TestSetup.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.Models.Features
{
    public class IDbPreventDeleteTests : BaseTestClass
    {
        public IDbPreventDeleteTests(WebOverrideFactory factory) : base(factory)
        {
        }

        [Theory, DefaultData]
        public async Task TestModifyAndDelete(LeaderboardModel leaderboard)
        {
            using var _ = CreateScope(out var scope);
            var ctx = scope.GetRequiredService<ApplicationDbContext>();
            var set = ctx.Leaderboards;

            // we don't care which division/uom we use for the purpose of this test
            leaderboard.DivisionId = (await ctx.Divisions.AsQueryable().SingleAsync(d => d.Id == "11a38bd5-49f1-4718-bd65-ab795da6fe26").ConfigureAwait(false)).Id;
            leaderboard.UOMId = (await ctx.UnitsOfMeasure.FirstAsync().ConfigureAwait(false)).Id;

            // add board
            await set.AddAsync(leaderboard).ConfigureAwait(false);
            await ctx.SaveChangesAsync().ConfigureAwait(false);

            Assert.Contains(leaderboard, set.WhereActive());

            // modify board
            leaderboard.Name = $"{leaderboard.Name} 2";
            set.Update(leaderboard);
            await ctx.SaveChangesAsync().ConfigureAwait(false);
            Assert.True(leaderboard.IsActive);

            // remove and refresh from the database.
            set.Remove(leaderboard);
            await ctx.SaveChangesAsync().ConfigureAwait(false);

            // IDbActive should set it to inactive rather than actually deleting it
            ctx.Entry(leaderboard).State = EntityState.Detached;
            leaderboard = await set.FindAsync(leaderboard.Id).ConfigureAwait(false);
            Assert.False(leaderboard.IsActive);

            // Sinces it's inactive, this should return nothing
            ctx.Entry(leaderboard).State = EntityState.Detached;
            leaderboard = await set.FindActiveAsync(leaderboard.Id).ConfigureAwait(false);
            Assert.Null(leaderboard);
        }
    }
}