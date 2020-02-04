using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using AutoFixture.Xunit2;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Tests.TestSetup;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.Models.Features
{
    public class IDbPreventDeleteTests : BaseTestClass
    {
        public IDbPreventDeleteTests(WebOverrideFactory factory) : base(factory)
        {
        }

        // TODO why don't userprofiles get tracked by auto history?

        [Theory, AutoData]
        public async Task TestModifyAndDelete(string leaderboardName)
            => await WithScopeAsync(async scope =>
            {
                var work = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var repo = work.GetRepository<LeaderboardModel>();

                var board = new LeaderboardModel {
                    Name = $"Board {leaderboardName}"
                };

                await repo.InsertAsync(board);

                await work.SaveChangesAsync();

                board.Name = $"{board.Name} 2";

                repo.Update(board);

                await work.SaveChangesAsync();

                Assert.True(board.IsActive);

                repo.Delete(board);

                await work.SaveChangesAsync();

                Assert.False(board.IsActive);
            });
    }
}