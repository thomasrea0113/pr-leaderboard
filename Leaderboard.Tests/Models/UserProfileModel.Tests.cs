using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Data;
using Leaderboard.Models.Relationships;
using Leaderboard.Tests.TestSetup;
using Leaderboard.Tests.TestSetup.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.Models
{
    public class ApplicationUserTests : BaseTestClass
    {

        public ApplicationUserTests(WebOverrideFactory factory) : base(factory)
        {
        }

        public async IAsyncEnumerable<ApplicationUser> AddUsersAsync(AppUserManager manager, IEnumerable<ApplicationUser> users)
        {
            foreach (var user in users)
            {
                yield return await AddUserAsync(manager, user, user.Email);
            }
        }
        public async Task<ApplicationUser> AddUserAsync(AppUserManager manager, ApplicationUser user)
            => await AddUserAsync(manager, user, user.Email);

        public async Task<ApplicationUser> AddUserAsync(AppUserManager manager, ApplicationUser user, string email)
        {
            user.Email = email;
            var result = await manager.CreateAsync(user);
            Assert.Empty(result.Errors);
            return user;
        }


        [Theory, DefaultData]
        public async Task TestCreateUsers(ApplicationUser[] users)
        {
            using var _ = CreateScope(out var scope);

            var manager = scope.GetRequiredService<AppUserManager>();

            // unsetting one of the emails to make sure it can be created
            users[1].Email = null;

            await foreach (var profile in AddUsersAsync(manager, users))
            {
                Assert.True(profile.IsActive);
            }
        }

        [Theory, DefaultData]
        public async Task TestDuplicateLeaderboard(LeaderboardModel[] leaderboards, ApplicationUser user)
        {
            using var _ = CreateScope(out var scope);

            var manager = scope.GetRequiredService<AppUserManager>();
            var ctx = scope.GetRequiredService<ApplicationDbContext>();

            var did = (await ctx.Divisions.AsQueryable().FirstAsync()).Id;
            var uomid = (await ctx.UnitsOfMeasure.AsQueryable().FirstAsync()).Id;
            foreach (var leaderboard in leaderboards)
            {
                leaderboard.DivisionId = did;
                leaderboard.UOMId = uomid;
            }

            // seed a user and some boards
            var profile = await AddUserAsync(manager, user, default);
            await ctx.Leaderboards.AddRangeAsync(leaderboards);
            await ctx.SaveChangesAsync();

            // create some test relationships
            var ub1 = UserLeaderboard.Create(profile, leaderboards[0]);
            var ub1Duplicate = UserLeaderboard.Create(profile, leaderboards[0]);
            var ub2 = UserLeaderboard.Create(profile, leaderboards[1]);

            // add the first and make sure it's the only one present
            await ctx.UserLeaderboards.AddAsync(ub1);
            await ctx.SaveChangesAsync();
            Assert.Single(profile.UserLeaderboards);

            // detach all and reload the profile. Now we have data in the DB that isn't
            // tracked locally
            foreach (var entry in ctx.ChangeTracker.Entries())
                entry.State = EntityState.Detached;
            profile = await manager.FindByIdAsync(profile.Id);

            // add the duplicate key. Will not fail because we aren't tracking it locally... I think.
            // I don't honestly know why this doesn't fail.
            await ctx.UserLeaderboards.AddAsync(ub1Duplicate);

            // I don't understand why this works. ChangeTracking is a mystery to me. Only thing
            // that matters is that after saving, it shows just the one
            Assert.Equal(1, profile.UserLeaderboards.Count);
            await ctx.SaveChangesAsync();
            Assert.Single(profile.UserLeaderboards);

            // add second ub, which should add successfully
            await ctx.UserLeaderboards.AddAsync(ub2);
            await ctx.SaveChangesAsync();

            Assert.Equal(2, profile.UserLeaderboards.Count);
        }
    }
}