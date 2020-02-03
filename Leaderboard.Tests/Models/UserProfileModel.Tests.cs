using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Leaderboard.Data;
using Leaderboard.Managers;
using Leaderboard.Models;
using Leaderboard.Models.Identity;
using Leaderboard.Models.Relationships;
using Leaderboard.Tests.TestSetup;
using Leaderboard.Tests.TestSetup.Fixtures;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
            var user = new IdentityUser<Guid>(userName);

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
        public async Task TestModifyProfile(string userName) => await WithScopeAsync(async scope =>
        {
            var manager = scope.ServiceProvider.GetRequiredService<UserProfileManager>();
            var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var profile = await AddUserAsync(manager, userName, default);
            Assert.Empty(profile.UserLeaderboards);

            var leaderboard = new LeaderboardModel {
                Name = "Test leaderboard"
            };

            profile.UserLeaderboards.Add(new UserLeaderboard {
                Leaderboard = leaderboard,
                User = profile
            });


            await manager.SaveChangesAsync();

            await ctx.Entry(profile).ReloadAsync();
            Assert.Contains(profile.UserLeaderboards, ul => ul.LeaderboardId == leaderboard.Id);

            profile.UserLeaderboards.Add(new UserLeaderboard());

            await ctx.SaveChangesAsync();

            await ctx.Entry(profile).ReloadAsync();
            Assert.Equal(1, profile.UserLeaderboards.Count);
        });
    }
}