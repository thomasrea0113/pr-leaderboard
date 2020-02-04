using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Leaderboard.Areas.Identity.Validators;
using Leaderboard.Managers;
using Leaderboard.Tests.TestSetup;
using Leaderboard.Tests.TestSetup.Fixtures;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.Models.Identity.Validators
{
    public class ModelValidationTests : BaseTestClass
    {
        public ModelValidationTests(WebOverrideFactory factory) : base(factory)
        {
        }

        [Theory, DefaultData]
        public async Task TestInvalidEmail(IdentityUser user) => await WithScopeAsync(async scope => {
            var manager = scope.GetRequiredService<UserProfileManager>();
            var describer = scope.GetRequiredService<IdentityErrorDescriber>();

            user.Email = "EHREHRHEdsdsdf92848****020394923949~!!";

            // first create the user
            var result = await manager.CreateAsync(user);

            Assert.Contains(result.Errors, e => e.Description == describer.InvalidEmail(user.Email).Description);
        });

        [Theory, DefaultData]
        public async Task TestBlankEmail(IdentityUser user) => await WithScopeAsync(async scope =>
        {
            user.Email = default;

            var manager = scope.GetRequiredService<UserProfileManager>();
            var result = await manager.CreateAsync(user);
            Assert.True(result.Succeeded);
        });

        [Theory, DefaultData]
        public async Task TestExistingEmail(IdentityUser[] users) => await WithScopeAsync(async scope =>
        {
            var manager = scope.GetRequiredService<UserProfileManager>();
            var describer = scope.GetRequiredService<IdentityErrorDescriber>();

            Assert.Contains(manager.UserValidators, u => u.GetType() == typeof(EmailNotRequiredValidator));

            // gives users same email
            users[1].Email = users[0].Email;

            // can create the first user with email;
            var result = await manager.CreateAsync(users[0]);
            Assert.Empty(result.Errors);

            result = await manager.CreateAsync(users[1]);

            // email is a duplicate, so should contain the duplicate email identity error
            Assert.Contains(result.Errors, e => e.Description == describer.DuplicateEmail(users[1].Email).Description);
            Assert.False(result.Succeeded);
        });
    }
}