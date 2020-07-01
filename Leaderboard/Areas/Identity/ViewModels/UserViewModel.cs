using System.Collections.Generic;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Leaderboards.ViewModels;

namespace Leaderboard.Areas.Identity.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public GenderValue? Gender { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public List<CategoryViewModel> Interests { get; set; }
        public List<LeaderboardViewModel> Leaderboards { get; set; }
    }
}
