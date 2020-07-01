using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Leaderboard.Areas.Identity.Managers;
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
        private readonly IMapper _mapper;

        public UsersController(AppUserManager userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<UserViewModel> Me()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var userViewModel = _mapper.Map<UserViewModel>(user);
            userViewModel.IsAdmin = await _userManager.IsInRoleAsync(user, "admin").ConfigureAwait(false);
            return userViewModel;
        }

        [HttpGet]
        [Route("")]
        [Authorize(Policy = "AppAdmin")]
        public IEnumerable<UserViewModel> Get(bool? isAdmin = null, bool? isActive = null)
        {
            var adminUserIds = _userManager.Users.Where(u =>
                u.UserRoles.Any(ur => ur.Role.NormalizedName == "ADMIN"))
                .Select(u => u.Id)
                .ToArray();

            var users = _mapper.ProjectTo<UserViewModel>(_userManager.Users);

            if (isActive != null)
                users = users.Where(u => u.IsActive == isActive);

            foreach (var user in users)
            {
                // setting admin on this user
                if (adminUserIds.Any(auid => auid == user.Id))
                    user.IsAdmin = true;

                // we are only pulling non-admin users, and this user is not an admin
                if (isAdmin == false && !user.IsAdmin)
                    yield return user;
                // we are only pulling admin users, and this user is an admin
                else if (isAdmin == true && user.IsAdmin)
                    yield return user;
                // isAdmin is null, so we are returning all users, whether or not they are an admin
                else if (isAdmin == null)
                    yield return user;
            }
        }
    }
}
