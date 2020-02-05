using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Areas.Profiles.Models;
using Leaderboard.Data;
using Leaderboard.Managers;
using Leaderboard.Models.Relationships;
using Leaderboard.Tests.TestSetup;
using Leaderboard.Tests.TestSetup.Fixtures;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.Models
{
    public class UserProfileModelTests : BaseTestClass
    {

        public UserProfileModelTests(WebOverrideFactory factory) : base(factory)
        {
        }

        public async IAsyncEnumerable<UserProfileModel> AddUsersAsync(UserProfileManager manager, IEnumerable<IdentityUser> users)
        {
            foreach (var user in users)
            {
                yield return await AddUserAsync(manager, user, user.Email);
            }
        }
        public async Task<UserProfileModel> AddUserAsync(UserProfileManager manager, IdentityUser user)
            => await AddUserAsync(manager, user, user.Email);

        public async Task<UserProfileModel> AddUserAsync(UserProfileManager manager, IdentityUser user, string email)
        {
            user.Email = email;

            var result = await manager.CreateAsync(user);

            Assert.Empty(result.Errors);

            var profile = await manager.GetProfileAsync(user);

            Assert.NotNull(profile);

            return profile;
        }


        [Theory, DefaultData]
        public async Task TestCreateUsers(IdentityUser[] users) => await WithScopeAsync(async scope =>
        {
            var manager = scope.GetRequiredService<UserProfileManager>();

            // unsetting one of the emails to make sure it can be created
            users[1].Email = null;

            await foreach (var profile in AddUsersAsync(manager, users))
            {
                Assert.NotNull(profile.User);
                Assert.Equal(profile.UserId, profile.User.Id);
                Assert.True(profile.IsActive);
            }

            await manager.SaveChangesAsync();
        });

        [Theory, DefaultData]
        public async Task TestDuplicateLeaderboardConnected(LeaderboardModel[] leaderboards, IdentityUser user)
            => await WithScopeAsync(async scope =>
        {
            var manager = scope.GetRequiredService<UserProfileManager>();
            var ctx = scope.GetRequiredService<ApplicationDbContext>();

            // seed a user and some boards
            var profile = await AddUserAsync(manager, user, default);
            await ctx.leaderboards.AddRangeAsync(leaderboards);
            await ctx.SaveChangesAsync();

            // create some test relationships
            var ub1 = UserLeaderboard.Create(profile, leaderboards[0]);
            var ub1Duplicate = UserLeaderboard.Create(profile, leaderboards[0]);
            var ub2 = UserLeaderboard.Create(profile, leaderboards[1]);

            // add the first and make sure it's the only one present
            await ctx.UserLeaderboards.AddAsync(ub1);
            await manager.SaveChangesAsync();
            Assert.Single(profile.UserLeaderboards);

            // add the duplicate key and wait for it to fail
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await ctx.UserLeaderboards.AddAsync(ub1Duplicate));

            Assert.Single(profile.UserLeaderboards);

            // even though the add threw an invalid operation exception, it still attached it to the context,
            // so we have to explicitly remove it
            ctx.Entry(ub1Duplicate).State = EntityState.Detached;

            // add second ub, which should add successfully
            await ctx.UserLeaderboards.AddAsync(ub2);
            await ctx.SaveChangesAsync();

            Assert.Equal(2, profile.UserLeaderboards.Count);
        });

        [Theory, DefaultData]
        public async Task TestDuplicateLeaderboardDisonnected(LeaderboardModel[] leaderboards, IdentityUser user)
            => await WithScopeAsync(async scope =>
        {
            var manager = scope.GetRequiredService<UserProfileManager>();
            var ctx = scope.GetRequiredService<ApplicationDbContext>();

            // seed a user and some boards
            var profile = await AddUserAsync(manager, user, default);
            await ctx.leaderboards.AddRangeAsync(leaderboards);
            await ctx.SaveChangesAsync();

            // create some test relationships
            var ub1 = UserLeaderboard.Create(profile, leaderboards[0]);
            var ub1Duplicate = UserLeaderboard.Create(profile, leaderboards[0]);
            var ub2 = UserLeaderboard.Create(profile, leaderboards[1]);

            // add the first and make sure it's the only one present
            await ctx.UserLeaderboards.AddAsync(ub1);
            await manager.SaveChangesAsync();
            Assert.Single(profile.UserLeaderboards);

            // detach all and reload the profile. Now we have data in the DB that isn't
            // tracked locally
            foreach (var entry in ctx.ChangeTracker.Entries())
                entry.State = EntityState.Detached;
            profile = await ctx.UserProfiles.FindAsync(profile.UserId);

            // add the duplicate key. Will not fail because we aren't tracking it locally... I think.
            // I don't honestly know why this doesn't fail.
            await ctx.UserLeaderboards.AddAsync(ub1Duplicate);

            // I don't understand why this works. ChangeTracking is a mystery to me. Only thing
            // that matters is that after saving, it shows just the one
            Assert.Equal(2, profile.UserLeaderboards.Count);
            await ctx.SaveChangesAsync();
            Assert.Single(profile.UserLeaderboards);

            // add second ub, which should add successfully
            await ctx.UserLeaderboards.AddAsync(ub2);
            await ctx.SaveChangesAsync();

            Assert.Equal(2, profile.UserLeaderboards.Count);
        });
    }
}