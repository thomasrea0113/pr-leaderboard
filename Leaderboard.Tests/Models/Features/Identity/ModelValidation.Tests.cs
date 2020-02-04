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

        [Theory, AutoData]
        public async Task TestInvalidEmail(string userName) => await WithScopeAsync(async scope => {
            var manager = scope.GetRequiredService<UserProfileManager>();
            var describer = scope.GetRequiredService<IdentityErrorDescriber>();

            var invalidEmail = "EHREHRHEdsdsdf92848****020394923949~!!";

            var user = new IdentityUser(userName) {
                Email = invalidEmail
            };

            // first create the user
            var result = await manager.CreateAsync(user);

            Assert.Contains(result.Errors, e => e.Description == describer.InvalidEmail(invalidEmail).Description);
        });

        [Theory, AutoData]
        public async Task TestBlankEmail(string userName) => await WithScopeAsync(async scope =>
        {
            var manager = scope.GetRequiredService<UserProfileManager>();
            var result = await manager.CreateAsync(new IdentityUser(userName));
            Assert.True(result.Succeeded);
        });

        [Theory, AutoData]
        public async Task TestExistingEmail(string[] userNames, string emailStr) => await WithScopeAsync(async scope =>
        {
            var manager = scope.GetRequiredService<UserProfileManager>();
            var describer = scope.GetRequiredService<IdentityErrorDescriber>();

            var email = $"{emailStr}@test.com";

            Assert.Contains(manager.UserValidators, u => u.GetType() == typeof(EmailNotRequiredValidator));

            // can create the first user with email;
            var result = await manager.CreateAsync(new IdentityUser(userNames[0]) {
                    Email = email
                });
            Assert.Empty(result.Errors);

            result = await manager.CreateAsync(new IdentityUser(userNames[1]) {
                    Email = email
                });

            // email is a duplicate, so should contain the duplicate email identity error
            Assert.Contains(result.Errors, e => e.Description == describer.DuplicateEmail(email).Description);
            Assert.False(result.Succeeded);
        });
    }
}