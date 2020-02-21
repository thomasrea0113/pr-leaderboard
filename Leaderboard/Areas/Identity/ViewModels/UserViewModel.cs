using System;
using System.Collections.Generic;
using System.Linq;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Leaderboards.Models;

namespace Leaderboard.Areas.Identity.ViewModels
{
    public class UserViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<Category> Interests { get; set; }
        public List<LeaderboardModel> Leaderboards { get; set; }

        public UserViewModel(ApplicationUser user)
        {
            UserName = user.UserName;
            Email = user.Email;
            Interests = user.UserCategories.Select(uc => uc.Category).ToList();
            Leaderboards = user.UserLeaderboards.Select(uc => uc.Leaderboard).ToList();
        }
    }
}
