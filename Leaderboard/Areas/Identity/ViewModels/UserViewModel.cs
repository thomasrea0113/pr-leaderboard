using System.Collections.Generic;
using System.Linq;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Leaderboards.ViewModels;

namespace Leaderboard.Areas.Identity.ViewModels
{
    public class UserViewModel
    {
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsAdmin { get; private set; }
        public List<CategoryViewModel> Interests { get; private set; }
        public List<LeaderboardViewModel> Leaderboards { get; private set; }

        public UserViewModel(ApplicationUser user, bool isAdmin = false)
        {
            UserName = user.UserName;
            Email = user.Email;
            IsActive = user.IsActive;
            IsAdmin = isAdmin;

            // if the passed in user was fully loaded (including all navigation properties)
            // go ahead and include the below collections
            Interests = user.UserCategories != null ?
                CategoryViewModel.Create(user.UserCategories.Select(uc => uc.Category)).ToList()
                : null;

            Leaderboards = user.UserLeaderboards != null ?
                LeaderboardViewModel.Create(user.UserLeaderboards.Select(uc => uc.Leaderboard)).ToList()
                : null;
        }
    }
}
