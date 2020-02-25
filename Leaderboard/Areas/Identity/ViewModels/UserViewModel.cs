using System;
using System.Collections.Generic;
using System.Linq;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Leaderboards.Models;

namespace Leaderboard.Areas.Identity.ViewModels
{
    public class UserViewModel
    {
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public List<Category> Interests { get; private set; }
        public List<LeaderboardModel> Leaderboards { get; private set; }

        public UserViewModel(ApplicationUser user)
        {
            UserName = user.UserName;
            Email = user.Email;
            Interests = user.UserCategories.Select(uc => uc.Category).ToList();
            Leaderboards = user.UserLeaderboards.Select(uc => uc.Leaderboard).ToList();
        }
    }
}
