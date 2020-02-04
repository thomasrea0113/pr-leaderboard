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

        public async IAsyncEnumerable<UserProfileModel> AddUsersAsync(UserProfileManager manager, Dictionary<string, string> users)
        {
            foreach ((var userName, var email) in users)
            {
                yield return await AddUserAsync(manager, userName, email);
            }
        }

        public async Task<UserProfileModel> AddUserAsync(UserProfileManager manager, string userName, string email)
        {
            var user = new IdentityUser(userName);

            if (email != default)
            {
                user.Email = email;
            }

            var result = await manager.CreateAsync(user);

            Assert.Empty(result.Errors);

            var profile = await manager.GetProfileAsync(user);

            Assert.NotNull(profile);

            return profile;
        }


        [Theory, AutoData]
        public async Task TestCreateUsers(Dictionary<string, string> userNames) => await WithScopeAsync(async scope =>
        {
            var manager = scope.ServiceProvider.GetRequiredService<UserProfileManager>();

            var withEmails = userNames.ToDictionary(str => str.Key, str => $"{str.Value}@test.com");

            // unsetting one of the emails to make sure it can be created
            withEmails[userNames.Keys.ElementAt(1)] = null;

            await foreach (var profile in AddUsersAsync(manager, withEmails))
            {
                // TODO why aren't properties lazy loaded?
                Assert.NotNull(profile.User);
                Assert.Equal(profile.UserId, profile.User.Id);
                Assert.True(profile.IsActive);
            }

            await manager.SaveChangesAsync();
        });


        [Theory, AutoData]
        public async Task TestModifyProfile(string[] leaderboardName, string userName) => await WithScopeAsync(async scope =>
        {
            var manager = scope.ServiceProvider.GetRequiredService<UserProfileManager>();
            var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var profile = await AddUserAsync(manager, userName, default);
            Assert.Empty(profile.UserLeaderboards);

            var leaderboard = new LeaderboardModel {
                Name = $"leaderboard {leaderboardName[0]}"
            };

            profile.UserLeaderboards.Add(new UserLeaderboard {
                Leaderboard = leaderboard,
                User = profile
            });


            await manager.SaveChangesAsync();

            await ctx.Entry(profile).ReloadAsync();
            Assert.Contains(profile.UserLeaderboards, ul => ul.LeaderboardId == leaderboard.Id);

            // should throw duplicate key exception in the database
            profile.UserLeaderboards.Add(new UserLeaderboard {
                Leaderboard = leaderboard,
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