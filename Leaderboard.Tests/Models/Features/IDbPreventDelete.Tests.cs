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
            => await WithScopeAsync(async scope =>
            {
                var work = scope.GetRequiredService<ApplicationDbContext>();
                var repo = work.leaderboards;

                // add board
                await repo.AddAsync(leaderboard);
                await work.SaveChangesAsync();

                Assert.Contains(leaderboard, repo.WhereActive());

                // modify board
                leaderboard.Name = $"{leaderboard.Name} 2";
                repo.Update(leaderboard);
                await work.SaveChangesAsync();
                Assert.True(leaderboard.IsActive);

                // remove and refresh from the database.
                repo.Remove(leaderboard);
                await work.SaveChangesAsync();

                // IDbActive should set it to inactive rather than actually deleting it
                work.Entry(leaderboard).State = EntityState.Detached;
                leaderboard = await repo.FindAsync(leaderboard.Id);
                Assert.False(leaderboard.IsActive);

                // Sinces it's inactive, this should return nothing
                work.Entry(leaderboard).State = EntityState.Detached;
                leaderboard = await repo.FindActiveAsync(leaderboard.Id);
                Assert.Null(leaderboard);
            });
    }
}