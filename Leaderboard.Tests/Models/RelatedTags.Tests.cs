using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Leaderboard.Data;
using Leaderboard.Models;
using Leaderboard.Tests.TestSetup;
using Leaderboard.Tests.TestSetup.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.Models
{
    public class RelatedTagTests : BaseTestClass
    {
        public RelatedTagTests(WebOverrideFactory factory) : base(factory)
        {
        }

        [Theory, DefaultData]
        public async Task TestRelatedTag(TagModel[] tags)
            => await WithScopeAsync(async scope =>
            {
                var ctx = scope.GetRequiredService<ApplicationDbContext>();

                await ctx.Tags.AddRangeAsync(tags);
            });
    }
}