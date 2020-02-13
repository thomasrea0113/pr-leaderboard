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
            leaderboard.DivisionId = (await ctx.Divisions.FirstAsync()).Id;
            leaderboard.UOMId = (await ctx.UnitsOfMeasure.FirstAsync()).Id;

            // add board
            await set.AddAsync(leaderboard);
            await ctx.SaveChangesAsync();

            Assert.Contains(leaderboard, set.WhereActive());

            // modify board
            leaderboard.Name = $"{leaderboard.Name} 2";
            set.Update(leaderboard);
            await ctx.SaveChangesAsync();
            Assert.True(leaderboard.IsActive);

            // remove and refresh from the database.
            set.Remove(leaderboard);
            await ctx.SaveChangesAsync();

            // IDbActive should set it to inactive rather than actually deleting it
            ctx.Entry(leaderboard).State = EntityState.Detached;
            leaderboard = await set.FindAsync(leaderboard.Id);
            Assert.False(leaderboard.IsActive);

            // Sinces it's inactive, this should return nothing
            ctx.Entry(leaderboard).State = EntityState.Detached;
            leaderboard = await set.FindActiveAsync(leaderboard.Id);
            Assert.Null(leaderboard);
        }
    }
}