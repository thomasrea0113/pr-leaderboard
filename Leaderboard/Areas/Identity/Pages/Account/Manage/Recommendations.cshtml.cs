using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Identity.ViewModels;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Areas.Identity.Pages.Account.Manage
{
    public class RecommendationsModel : PageModel
    {
        private readonly AppUserManager _manager;

        public class ReactProps {
            public string InitialUrl { get; set; }
        }

        public class ReactState {
            public UserViewModel User { get; set; }
            public IEnumerable<LeaderboardModel> UserBoards { get; set; }
            public IEnumerable<LeaderboardModel> Recommendations { get; set; }
        }

        public ReactProps Props { get; set; } = new ReactProps();

        private string GetInitialUrl() => Url.Page("", new { handler = "initial" });

        public RecommendationsModel(AppUserManager manager)
        {
            _manager = manager;
        }

        public void OnGet()
        {
            Props.InitialUrl = GetInitialUrl();
        }

        public async Task<JsonResult> OnGetInitialAsync()
        {
            var user = await _manager.GetCompleteUser(User);
            var userBoards = user.UserLeaderboards.Select(ub => ub.Leaderboard);
            var recommendations = await _manager.GetRecommendedBoardsAsync(user);
            return new JsonResult(new ReactState
            {
                User = new UserViewModel(user),
                UserBoards = userBoards,
                Recommendations = recommendations.Except(userBoards)
            });
        }
    }
}