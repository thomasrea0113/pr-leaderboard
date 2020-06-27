using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Identity.ViewModels;
using Leaderboard.Tests.TestSetup;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.Api
{
    public class UsersControllerTests : BaseTestClass
    {
        public UsersControllerTests(WebOverrideFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// a user view comparer that only cares about relavent fields to the specific tests
        /// </summary>
        private class UserViewModelComparer : EqualityComparer<UserViewModel>
        {
            public override bool Equals([AllowNull] UserViewModel x, [AllowNull] UserViewModel y)
                => x.UserName == y.UserName && x.IsAdmin == y.IsAdmin && x.IsActive == y.IsActive;

            public override int GetHashCode([DisallowNull] UserViewModel obj)
            {
                unchecked
                {
                    int hash = 17;
                    // Suitable nullity checks etc, of course :)
                    hash = hash * 23 + obj.UserName.GetHashCode();
                    hash = hash * 23 + obj.IsAdmin.GetHashCode();
                    hash = hash * 23 + obj.IsActive.GetHashCode();
                    return hash;
                }
            }
        }

        [Fact]
        public async Task TestAll()
        {
            using var _ = CreateScope(out var scope);

            var users = scope.GetRequiredService<UsersController>();

            var activeAdminUsers = (await scope.GetRequiredService<AppUserManager>()
                .GetUsersInRoleAsync("admin").ConfigureAwait(false))
                .Where(u => u.IsActive)
                .Select(u => new UserViewModel(u, true))
                .OrderBy(u => u.UserName)
                .ToList();

            var allActiveUsers = (await users.All(isActive: true).ConfigureAwait(false))
                .OrderBy(u => u.UserName)
                .ToList();

            var allActiveAdminUsers = allActiveUsers.Where(u => u.IsAdmin)
                .OrderBy(u => u.UserName)
                .ToList();

            var comparer = new UserViewModelComparer();

            // make sure the admin users aren't just the same set of users
            Assert.NotEqual(allActiveAdminUsers, allActiveUsers, comparer);

            // users returned by Controller must equal users in the admin role
            Assert.Equal(allActiveAdminUsers, activeAdminUsers, comparer);
        }
    }
}