using System;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Managers;
using Leaderboard.Models.Identity.Validators;
using Leaderboard.Tests.TestSetup.Fixtures;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Leaderboard.Tests.Models.Identity.Validators
{
    public class EmailNotRequiredValidatorTests
    {
        private readonly EmailNotRequiredValidator _validator = new EmailNotRequiredValidator();
        private readonly Type _validatorType = typeof(EmailNotRequiredValidator);
        private readonly Type _EmailExistsIdentityErrorType = typeof(EmailExistsIdentityError);

        [Theory, DefaultData]
        public async Task TestBlankEmail(UserManager<IdentityUser<Guid>> manager)
        {
            Assert.Equal(0, await manager.Users.CountAsync());

            // can create the first user with email;
            var result = await manager.CreateAsync(new IdentityUser<Guid>("user1"));
            Assert.True(result.Succeeded);
        }

        [Theory, DefaultData]
        public async Task TestExistingEmail(UserProfileManager manager)
        {
            var email = "test.user@test.com";

            Assert.Equal(0, await manager.Users.CountAsync());

            Assert.Contains(manager.UserValidators, u => u.GetType() == _validatorType); 

            // can create the first user with email;
            var result = await manager.CreateAsync(new IdentityUser<Guid>("user1") {
                Email = email
            });
            Assert.True(result.Succeeded);

            Assert.Equal(1, await manager.Users.CountAsync());

            result = await manager.CreateAsync(new IdentityUser<Guid>("user2") {
                Email = email
            });

            Assert.Equal(1, await manager.Users.CountAsync());

            // email is a duplicate, so should contain the duplicate email identity error
            Assert.Contains(result.Errors, e => e.GetType() == _EmailExistsIdentityErrorType);
            Assert.False(result.Succeeded);
        }
    }
}