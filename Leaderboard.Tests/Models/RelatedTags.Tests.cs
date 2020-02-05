using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Leaderboard.Data;
using Leaderboard.Models;
using Leaderboard.Models.Relationships;
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
                await ctx.SaveChangesAsync();
                Assert.All(tags, t => Assert.Empty(t.RelatedTags));

                // first tag
                var last = tags.Skip(2).Select(t => new RelatedTag {
                    Tag = tags[0],
                    Related = t
                }).Single();

                // last two tags
                var firstTwo = tags.Take(2).Select(t => new RelatedTag {
                    Tag = tags[2],
                    Related = t
                }).ToList();

                // IMPORTANT NOTE - with lazy loading, EF will automatically update
                // any property of the matching type. Because RelatedTags has two
                // TagModel properties, BOTH will be set to the current tag that
                // you are appending. This is nice in general, but that means now
                // that we must add the relationship directly, and only use the list properties
                // on the model for reading.

                // tags[0].RelatedTags.AddRange(last);
                // tags[2].RelatedTags.AddRange(firstTwo);
                await ctx.RelatedTags.AddRangeAsync(firstTwo.Append(last).ToArray());

                // FIXME onsave, it replaces the RelatedId with the TagId
                await ctx.SaveChangesAsync();

                Assert.NotEqual(last.TagId, last.RelatedId);

                foreach (var tag in tags)
                    await ctx.Entry(tag).ReloadAsync();

                Assert.Single(tags[0].RelatedTags);
                Assert.Empty(tags[1].RelatedTags);
                Assert.Single(tags[2].RelatedTags);

                var rt = tags[0].RelatedTags.Single();
                Assert.Equal(last.TagId, rt.TagId);
                Assert.Equal(last.RelatedId, rt.RelatedId);
                Assert.All(tags[2].RelatedTags, tr => firstTwo.Any(r => r.TagId == tr.TagId && r.RelatedId == tr.RelatedId));

                Assert.Single(tags[0].RelatedToMeTags);
                Assert.Equal(2, tags[0].RelatedToMeTags.Count);
                Assert.Single(tags[2].RelatedToMeTags);
            });
    }
}