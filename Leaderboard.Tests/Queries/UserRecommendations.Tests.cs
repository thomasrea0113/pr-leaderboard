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
            var recommendations = await um.GetRecommendedBoardsAsync(user);
        }
    }
}