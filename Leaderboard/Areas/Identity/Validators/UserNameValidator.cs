using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Leaderboard.Areas.Identity.Validators
{
    public class UserNameValidator : IUserValidator<ApplicationUser>
    {
        public UserNameValidator(IdentityErrorDescriber describer)
        {
            _describer = describer;
        }

        private readonly Regex _validUserName = new Regex(".*[a-zA-Z0-9]$");
        private readonly IdentityErrorDescriber _describer;

        public async Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            await Task.CompletedTask;
            if (!_validUserName.IsMatch(user.UserName))
                return IdentityResult.Failed(_describer.InvalidUserName(user.UserName));
            return IdentityResult.Success;
        }
    }
}
