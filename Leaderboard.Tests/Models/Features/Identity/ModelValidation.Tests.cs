using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Identity.Validators;
using Leaderboard.Tests.TestSetup;
using Leaderboard.Tests.TestSetup.Fixtures;
using Microsoft.AspNetCore.Identity;
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
        public async Task TestInvalidEmail(ApplicationUser user)
        {
            using var _ = CreateScope(out var scope);

            var manager = scope.GetRequiredService<AppUserManager>();
            var describer = scope.GetRequiredService<IdentityErrorDescriber>();

            user.Email = "EHREHRHEdsdsdf92848****020394923949~!!";

            // first create the user
            var result = await manager.CreateAsync(user).ConfigureAwait(false);

            Assert.Contains(result.Errors, e => e.Description == describer.InvalidEmail(user.Email).Description);
        }

        [Theory, DefaultData]
        public async Task TestBlankEmail(ApplicationUser user)
        {
            using var _ = CreateScope(out var scope);

            user.Email = default;

            var manager = scope.GetRequiredService<AppUserManager>();
            var result = await manager.CreateAsync(user).ConfigureAwait(false);
            Assert.True(result.Succeeded);
        }

        [Theory, DefaultData]
        public async Task TestExistingEmail(ApplicationUser[] users)
        {
            using var _ = CreateScope(out var scope);

            var manager = scope.GetRequiredService<AppUserManager>();
            var describer = scope.GetRequiredService<IdentityErrorDescriber>();

            Assert.Contains(manager.UserValidators, u => u.GetType() == typeof(EmailNotRequiredValidator));

            // gives users same email
            users[1].Email = users[0].Email;

            // can create the first user with email;
            var result = await manager.CreateAsync(users[0]).ConfigureAwait(false);
            Assert.Empty(result.Errors);

            result = await manager.CreateAsync(users[1]).ConfigureAwait(false);

            // email is a duplicate, so should contain the duplicate email identity error
            Assert.Contains(result.Errors, e => e.Description == describer.DuplicateEmail(users[1].Email).Description);
            Assert.False(result.Succeeded);
        }
    }
}