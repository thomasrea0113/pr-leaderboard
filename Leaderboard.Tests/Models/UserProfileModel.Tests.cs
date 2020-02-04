using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using AutoFixture.Xunit2;
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
        public async Task TestModifyProfile(LeaderboardModel[] leaderboards, IdentityUser user)
            => await WithScopeAsync(async scope =>
        {
            var manager = scope.GetRequiredService<UserProfileManager>();
            var ctx = scope.GetRequiredService<ApplicationDbContext>();

            var profile = await AddUserAsync(manager, user, default);
            Assert.Empty(profile.UserLeaderboards);

            profile.UserLeaderboards.Add(new UserLeaderboard {
                Leaderboard = leaderboards[0],
                User = profile
            });


            await manager.SaveChangesAsync();

            await ctx.Entry(profile).ReloadAsync();
            Assert.Contains(profile.UserLeaderboards, ul => ul.LeaderboardId == leaderboards[0].Id);

            // should throw duplicate key exception in the database
            profile.UserLeaderboards.Add(new UserLeaderboard {
                Leaderboard = leaderboards[0],
                User = profile
            });
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await ctx.SaveChangesAsync());

            // should throw exception because leaderbard is null
            profile.UserLeaderboards.Add(new UserLeaderboard {
                Leaderboard = null,
                User = profile
            });
            await Assert.ThrowsAsync<DbUpdateException>(async () => await ctx.SaveChangesAsync());

            // NOTE you cannot create related objects like below - the object must already exist
            // profile.UserLeaderboards.Add(new UserLeaderboard {
            //     Leaderboard = new LeaderboardModel { Name = leaderboardName[1] },
            //     User = profile
            // });
            // await ctx.SaveChangesAsync();

            await ctx.Entry(profile).ReloadAsync();

            // FIXME for some reason, the null Leaderboard doesn't prevent the relationship from being saved.
            // somewhere, the LeaderboardId is being set
            Assert.Equal(2, profile.UserLeaderboards.Count);
        });
    }
}