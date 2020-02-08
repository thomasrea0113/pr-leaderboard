using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Leaderboard.Areas.Leaderboards.Models;
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
    public class RelatedDivisionTests : BaseTestClass
    {
        public RelatedDivisionTests(WebOverrideFactory factory) : base(factory)
        {
        }

        private async IAsyncEnumerable<Division> ReloadTagsAsync(DbContext ctx, params Division[] tags)
        {
            foreach (var tag in tags)
            {
                ctx.Entry(tag).State = EntityState.Detached;
                var found = await ctx.Set<Division>().FindAsync(tag.Id);
                await ctx.Entry(found).Collection(f => f.RelatedDivisions).LoadAsync();
                await ctx.Entry(found).Collection(f => f.DivisionsRelatedToMe).LoadAsync();
                yield return found;
            }
        }

        [Theory, DefaultData]
        public async Task TestRelatedDivision(Division[] tags)
            => await WithScopeAsync(async scope =>
            {
                var ctx = scope.GetRequiredService<ApplicationDbContext>();

                // add the initial tags
                await ctx.Divisions.AddRangeAsync(tags);
                await ctx.SaveChangesAsync();

                // setting up the tag relationships
                var last = tags
                    .Skip(2).Select(t => RelatedDivision.Create(tags[0], t))
                    .Single(); // last tag - added to tag 0
                var firstTwo = tags.Take(2)
                    .Select(t => RelatedDivision.Create(tags[2], t))
                    .ToList(); // first two tags - added to tag 2
                var allTags = firstTwo.Append(last)
                    .Distinct()
                    .ToArray(); // all tag relationships

                // add all relationships
                await ctx.RelatedDivisions.AddRangeAsync(allTags);
                await ctx.SaveChangesAsync();

                // confirming the related tags were set
                Assert.Equal(last, tags[0].RelatedDivisions.Single());
                Assert.Empty(tags[1].RelatedDivisions);
                Assert.Equal(firstTwo, tags[2].RelatedDivisions);

                /*
                Asserting the RelatedToMe - this is the reverse of a related tag.
                When looking at what's related to you, you are the related. And the
                tag is what points to you. You can take the union of both collections
                to get everything that 'I have said I'm related to, and everything that
                says it's related to me'
                */

                Assert.Equal(tags[2], tags[0].DivisionsRelatedToMe.Single().Division);
                Assert.Equal(tags[0], tags[0].DivisionsRelatedToMe.Single().Related);

                Assert.Equal(tags[2], tags[1].DivisionsRelatedToMe.Single().Division);
                Assert.Equal(tags[1], tags[1].DivisionsRelatedToMe.Single().Related);

                Assert.Equal(tags[0], tags[2].DivisionsRelatedToMe.Single().Division);
                Assert.Equal(tags[2], tags[2].DivisionsRelatedToMe.Single().Related);

                /*
                tag 0 has a Related and ReleatedToMe tag, however they are
                the same. So the union performed by AllRelatedDivisions returns one.
                */

                Assert.Single(tags[0].AllRelatedDivisions);
                Assert.Single(tags[1].AllRelatedDivisions);
                Assert.Equal(2, tags[2].AllRelatedDivisions.Count);
            });
    }
}