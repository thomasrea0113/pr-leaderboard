using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Identity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Areas.Identity
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppUserManager _userManager;

        public UsersController(AppUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<UserViewModel> Me()
        {
            var user = await _userManager.GetUserAsync(User);
            var userViewModel = new UserViewModel(user, await _userManager.IsInRoleAsync(user, "admin"));
            return userViewModel;
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize(Roles = "admin")]
        public async Task<IEnumerable<UserViewModel>> All(bool? isAdmin = null, bool? isActive = null)
        {
            var users = _userManager.Users
                .Select(u => new { IsAdmin = u.UserRoles.Any(u2 => u2.Role.NormalizedName == "ADMIN"), u });

            if (isActive != null)
                users = users.Where(u => u.u.IsActive == isActive);

            if (isAdmin != null)
                users = users.Where(u => u.IsAdmin == isAdmin);


            var userViewModels = (await users.ToArrayAsync())
                // if isAdmin is defined, we know we're only returning users with that flag
                // otherwise, return the user's admin state
                .Select(u => new UserViewModel(u.u, isAdmin ?? u.IsAdmin));

            return userViewModels;
        }
    }
}
