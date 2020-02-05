using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Leaderboard.Data;
using Leaderboard.Models;
using Leaderboard.Models.Relationships;
using Leaderboard.Tests.TestSetup;
using Leaderboard.Tests.TestSetup.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.Models
{
    public class RelatedTagTests : BaseTestClass
    {
        public RelatedTagTests(WebOverrideFactory factory) : base(factory)
        {
        }

        private async IAsyncEnumerable<TagModel> ReloadTagsAsync(DbContext ctx, params TagModel[] tags)
        {
            foreach (var tag in tags)
            {
                ctx.Entry(tag).State = EntityState.Detached;
                var found = await ctx.Set<TagModel>().FindAsync(tag.Id);
                await ctx.Entry(found).Collection(f => f.RelatedTags).LoadAsync();
                await ctx.Entry(found).Collection(f => f.RelatedToMeTags).LoadAsync();
                yield return found;
            }
        }

        [Theory, DefaultData]
        public async Task TestRelatedTag(TagModel[] tags)
            => await WithScopeAsync(async scope =>
            {
                var ctx = scope.GetRequiredService<ApplicationDbContext>();

                // add the initial tags
                await ctx.Tags.AddRangeAsync(tags);
                await ctx.SaveChangesAsync();

                // setting up the tag relationships
                var last = tags
                    .Skip(2).Select(t => RelatedTag.Create(tags[0], t))
                    .Single(); // last tag - added to tag 0
                var firstTwo = tags.Take(2)
                    .Select(t => RelatedTag.Create(tags[2], t))
                    .ToList(); // first two tags - added to tag 2
                var allTags = firstTwo.Append(last)
                    .Distinct()
                    .ToArray(); // all tag relationships

                // add all relationships
                await ctx.RelatedTags.AddRangeAsync(allTags);
                await ctx.SaveChangesAsync();

                // confirming the related tags were set
                Assert.Equal(last, tags[0].RelatedTags.Single());
                Assert.Empty(tags[1].RelatedTags);
                Assert.Equal(firstTwo, tags[2].RelatedTags);

                /*
                Asserting the RelatedToMe - this is the reverse of a related tag.
                When looking at what's related to you, you are the related. And the
                tag is what points to you. You can take the union of both collections
                to get everything that 'I have said I'm related to, and everything that
                says it's related to me'
                */

                Assert.Equal(tags[2], tags[0].RelatedToMeTags.Single().Tag);
                Assert.Equal(tags[0], tags[0].RelatedToMeTags.Single().Related);

                Assert.Equal(tags[2], tags[1].RelatedToMeTags.Single().Tag);
                Assert.Equal(tags[1], tags[1].RelatedToMeTags.Single().Related);

                Assert.Equal(tags[0], tags[2].RelatedToMeTags.Single().Tag);
                Assert.Equal(tags[2], tags[2].RelatedToMeTags.Single().Related);

                /*
                tag 0 has a Related and ReleatedToMe tag, however they are
                the same. So the union performed by AllRelatedTags returns one.
                */

                Assert.Single(tags[0].AllRelatedTags);
                Assert.Single(tags[1].AllRelatedTags);
                Assert.Equal(2, tags[2].AllRelatedTags.Count);
            });
    }
}