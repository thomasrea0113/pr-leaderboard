using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly Dictionary<string, string> _users = new Dictionary<string, string>() {
            { "user1", "user1.email@test.com" },
            { "user2", "user2.email@test.com" },
            { "user3-no-email", default },
            { "user4", "user4.email@test.com" },
        };

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


        [Fact]
        public async Task TestCreateUsers() => await WithScopeAsync(async scope =>
        {
            var manager = scope.ServiceProvider.GetRequiredService<UserProfileManager>();

            await foreach (var profile in AddUsersAsync(manager, _users))
            {
                Assert.NotNull(profile.User);
                Assert.Equal(profile.UserId, profile.User.Id);
                Assert.True(profile.IsActive);
            }

            await manager.SaveChangesAsync();
        });
    }
}