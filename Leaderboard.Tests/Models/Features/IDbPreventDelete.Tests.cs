using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using AutoFixture.Xunit2;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Tests.TestSetup;
using Leaderboard.Tests.TestSetup.Fixtures;
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
                var work = scope.GetRequiredService<IUnitOfWork>();
                var repo = work.GetRepository<LeaderboardModel>();

                await repo.InsertAsync(leaderboard);

                await work.SaveChangesAsync();

                leaderboard.Name = $"{leaderboard.Name} 2";

                repo.Update(leaderboard);

                await work.SaveChangesAsync();

                Assert.True(leaderboard.IsActive);

                repo.Delete(leaderboard);

                await work.SaveChangesAsync();

                Assert.False(leaderboard.IsActive);
            });
    }
}