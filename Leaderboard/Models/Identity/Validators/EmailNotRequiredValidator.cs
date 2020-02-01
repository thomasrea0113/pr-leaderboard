using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Models.Identity.Validators
{
    public class EmailExistsIdentityError : IdentityError
    {
        public EmailExistsIdentityError(string email)
        {
            this.Code = "DuplicateEmail";
            this.Description = $"Email '{email}' is already taken.";
        }
    }

    /// <summary>
    /// Allows the user email field to be left blank. However, if it is supplied,
    /// it must be unique in the database
    /// </summary>
    public class EmailNotRequiredValidator : IUserValidator<IdentityUser<Guid>>
    {
        public async Task<IdentityResult> ValidateAsync(UserManager<IdentityUser<Guid>> manager, IdentityUser<Guid> user)
        {
            var emailExists = await manager.Users.AnyAsync(u => u.Email == user.Email);

            if (emailExists)
                return IdentityResult.Failed(new EmailExistsIdentityError(user.Email));

            return IdentityResult.Success;
        }
    }
}