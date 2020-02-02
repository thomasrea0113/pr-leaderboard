using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Leaderboard.Managers;
using Leaderboard.Models.Identity;
using Leaderboard.Tests.TestSetup;
using Leaderboard.Tests.TestSetup.Fixtures;
using Microsoft.AspNetCore.Identity;
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
                var user = new IdentityUser<Guid>(userName);

                if (email != default)
                {
                    user.Email = email;
                }

                var profileModel = new UserProfileModel() {
                    User = user
                };

                var result = await manager.AddProfileAsync(profileModel);

                Assert.Empty(result.Errors);

                yield return profileModel;
            }
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
                Assert.NotNull(profile.User);
                Assert.Equal(profile.UserId, profile.User.Id);
                Assert.True(profile.IsActive);
            }

            await manager.SaveChangesAsync();
        });
    }
}