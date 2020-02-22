using System;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Data;
using Leaderboard.Tests.TestSetup;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.Queries
{
    public class UserRecommendationsTests : BaseTestClass
    {
        public UserRecommendationsTests(WebOverrideFactory factory) : base(factory)
        {
        }

        // FIXME for some reason, tests have to be rerun a few times for them to work.
        // this issue has something to do with the fact that the database is created/migrated/seeded
        // on initial startup if there are pending migrations
        /// <summary>
        /// Tests the recommendations for a user with both a weight and an age
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestWithAgeAndWeight()
        {
            using var _ = CreateScope(out var scope);
            var um = scope.GetRequiredService<AppUserManager>();

            // this user is in the powerlifting divisions, which have a weight and an age
            var user = await um.FindByNameAsync("LifterDuder");

            Assert.NotEmpty(user.UserCategories);

            var recommendations = await um.GetRecommendedBoardsAsync(user);

            Assert.NotEmpty(recommendations);
        }
    }
}