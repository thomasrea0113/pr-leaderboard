using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Identity.ViewModels;
using Leaderboard.Areas.Leaderboards.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Areas.Identity.Pages.Account.Manage
{
    public class ReactProps
    {
        public string InitialUrl { get; set; }
        public string UserName { get; set; }
    }

    public class ReactState
    {
        public UserViewModel User { get; set; }
        public IEnumerable<UserLeaderboardViewModel> Recommendations { get; set; }
    }
    public class RecommendationsModel : PageModel
    {
        private readonly AppUserManager _manager;

        public ReactProps Props { get; set; } = new ReactProps();

        private string GetInitialUrl() => Url.Page("", new { handler = "initial" });

        public RecommendationsModel(AppUserManager manager)
        {
            _manager = manager;
        }

        public void OnGet()
        {
            Props.InitialUrl = GetInitialUrl();
            Props.UserName = User.FindFirst(ClaimTypes.Name).Value;
        }

        public async Task<JsonResult> OnGetInitialAsync()
        {
            var user = await _manager.GetCompleteUserAsync(User).ConfigureAwait(false);
            var userBoards = user.UserLeaderboards.Select(ub => ub.Leaderboard);

            var recommendations = await _manager.GetRecommendedBoardsQuery(user)
                .Include(b => b.Division)
                .ToArrayAsync().ConfigureAwait(false);

            var allUserBoards = UserLeaderboardViewModel
                .Create(userBoards, true, false, Url)
                .Concat(UserLeaderboardViewModel
                    .Create(recommendations.Except(userBoards), false, true, Url)).Distinct();

            return new JsonResult(new ReactState
            {
                User = new UserViewModel(user),
                Recommendations = allUserBoards
            });
        }
    }
}