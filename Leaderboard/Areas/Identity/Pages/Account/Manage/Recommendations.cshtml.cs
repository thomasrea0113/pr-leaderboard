using System.Collections.Generic;
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
            public List<LeaderboardModel> Recommendations { get; set; }
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
            return new JsonResult(new ReactState
            {
                User = new UserViewModel(user),
                Recommendations = await _manager.GetRecommendedBoardsAsync(user)
            });
        }
    }
}