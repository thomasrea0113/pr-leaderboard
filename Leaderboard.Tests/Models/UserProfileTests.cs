using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Leaderboard.Tests.TestSetup.Fixtures;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace Leaderboard.Tests.Models
{
    public class UserProfileTests
    {
        private readonly Dictionary<string, string> _users = new Dictionary<string, string>() {
            { "user1", "user1.email@test.com" },
            { "user2", "user2.email@test.com" },
            { "user3-no-email", default },
            { "user4", "user4.email@test.com" },
        };

        public async IAsyncEnumerable<ValueTuple<IdentityResult, IdentityUser<Guid>>> CreateUsersAsync(UserManager<IdentityUser<Guid>> manager, Dictionary<string, string> users)
        {
            foreach ((var userName, var email) in users)
            {
                var user = new IdentityUser<Guid>(userName);

                if (email != default)
                {
                    user.Email = email;
                }

                yield return (await manager.CreateAsync(user), user);
            }
        }


        [Theory, DefaultData("Users")]
        public async Task TestCreateUsers(UserManager<IdentityUser<Guid>> manager)
        {
            await foreach ((var result, var user) in CreateUsersAsync(manager, _users))
            {
                Assert.Empty(result.Errors);
            }
        }
    }
}