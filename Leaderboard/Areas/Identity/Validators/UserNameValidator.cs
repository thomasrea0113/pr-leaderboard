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

        public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            if (!_validUserName.IsMatch(user.UserName))
                return Task.FromResult(IdentityResult.Failed(_describer.InvalidUserName(user.UserName)));
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
