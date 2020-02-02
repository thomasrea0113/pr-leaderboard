using System;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Managers;
using Leaderboard.Models.Identity.Validators;
using Leaderboard.Tests.TestSetup;
using Leaderboard.Tests.TestSetup.Fixtures;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.Models.Identity.Validators
{
    public class EmailNotRequiredValidatorTests : BaseTestClass
    {
        private readonly EmailNotRequiredValidator _validator = new EmailNotRequiredValidator();
        private readonly Type _validatorType = typeof(EmailNotRequiredValidator);
        private readonly Type _EmailExistsIdentityErrorType = typeof(EmailExistsIdentityError);

        public EmailNotRequiredValidatorTests(WebOverrideFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task TestBlankEmail() => await WithScopeAsync(async scope =>
        {
            var manager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();

            Assert.Equal(0, await manager.Users.CountAsync());

            // can create the first user with email;
            var result = await manager.CreateAsync(new IdentityUser<Guid>("user0"));
            Assert.True(result.Succeeded);
        });

        [Fact]
        public async Task TestExistingEmail() => await WithScopeAsync(async scope =>
        {
            var manager = scope.ServiceProvider.GetRequiredService<UserProfileManager>();

            var email = "test.user@test.com";

            var count = await manager.Users.CountAsync();

            Assert.Contains(manager.UserValidators, u => u.GetType() == _validatorType);

            // can create the first user with email;
            var result = await manager.CreateAsync(new IdentityUser<Guid>("user6")
            {
                Email = email
            });
            Assert.True(result.Succeeded);

            Assert.Equal(count + 1, await manager.Users.CountAsync());

            result = await manager.CreateAsync(new IdentityUser<Guid>("user7")
            {
                Email = email
            });

            Assert.Equal(count + 1, await manager.Users.CountAsync());

            // email is a duplicate, so should contain the duplicate email identity error
            Assert.Contains(result.Errors, e => e.GetType() == _EmailExistsIdentityErrorType);
            Assert.False(result.Succeeded);
        });
    }
}