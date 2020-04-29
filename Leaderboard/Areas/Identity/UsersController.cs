using System;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Identity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}
