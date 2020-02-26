using System;
using System.Collections.Generic;
using System.Linq;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Areas.Leaderboards.ViewModels;

namespace Leaderboard.Areas.Identity.ViewModels
{
    public class UserViewModel
    {
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public List<CategoryViewModel> Interests { get; private set; }
        public List<LeaderboardModel> Leaderboards { get; private set; }

        public UserViewModel(ApplicationUser user)
        {
            UserName = user.UserName;
            Email = user.Email;
            Interests = CategoryViewModel.Create(user.UserCategories.Select(uc => uc.Category).ToArray()).ToList();
            Leaderboards = user.UserLeaderboards.Select(uc => uc.Leaderboard).ToList();
        }
    }
}
