using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Models.Identity.Validators
{
    /// <summary>
    /// Allows the user email field to be left blank. However, if it is supplied,
    /// it must be unique in the database
    /// </summary>
    public class EmailNotRequiredValidator : IUserValidator<IdentityUser>
    {
        public const string EmailRegexString = "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])";

        public async Task<IdentityResult> ValidateAsync(UserManager<IdentityUser> manager, IdentityUser user)
        {
            var errorDescriber = new IdentityErrorDescriber();

            if (user.Email != default)
            {
                if (!Regex.IsMatch(user.Email, EmailRegexString))
                    return IdentityResult.Failed(errorDescriber.InvalidEmail(user.Email));

                var emailExists = await manager.Users.AnyAsync(u => u.Email == user.Email);

                if (emailExists)
                    return IdentityResult.Failed(errorDescriber.DuplicateEmail(user.Email));
            }

            return IdentityResult.Success;
        }
    }
}