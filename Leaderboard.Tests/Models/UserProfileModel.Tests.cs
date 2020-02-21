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
    }
}