using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Leaderboard.Managers;
using Leaderboard.Models.Identity;
using Leaderboard.Tests.TestSetup;
using Leaderboard.Tests.TestSetup.Fixtures;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.Models
{
    public class UserProfileTests : BaseTestClass
    {
        private readonly Dictionary<string, string> _users = new Dictionary<string, string>() {
            { "user1", "user1.email@test.com" },
            { "user2", "user2.email@test.com" },
            { "user3-no-email", default },
            { "user4", "user4.email@test.com" },
        };

        public UserProfileTests(WebOverrideFactory factory) : base(factory)
        {
        }

        public async IAsyncEnumerable<ValueTuple<IdentityResult, UserProfileModel>> CreateUsersAsync(UserProfileManager manager, Dictionary<string, string> users)
        {
            foreach ((var userName, var email) in users)
            {
                var user = new IdentityUser<Guid>(userName);

                if (email != default)
                {
                    user.Email = email;
                }

                var result = await manager.CreateAsync(user);

                var profile = await manager.GetOrCreateProfileAsync(user);

                // all new users, so all new profiles
                Assert.True(profile.Item1);

                yield return (result, profile.Item2);
            }
        }


        [Fact]
        public async Task TestCreateUsers()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var manager = scope.ServiceProvider.GetRequiredService<UserProfileManager>();

                await foreach ((var result, var user) in CreateUsersAsync(manager, _users))
                {
                    Assert.Empty(result.Errors);
                    Assert.NotNull(user);
                    Assert.True(user.IsActive);
                    Assert.NotNull(user.User);
                }
            }
        }
    }
}