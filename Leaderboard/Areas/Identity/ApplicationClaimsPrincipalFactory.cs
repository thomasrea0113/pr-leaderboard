using System.Security.Claims;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Leaderboard.Areas.Identity
{
    /// <summary>
    /// Add additional claims to the user identity
    /// </summary>
    public class ApplicationClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        public ApplicationClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<IdentityOptions> options) : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user).ConfigureAwait(false);
            if (user.Email != null)
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            return identity;
        }
    }
}