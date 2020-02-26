using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Identity.ViewModels;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Areas.Leaderboards.ViewModels;
using Leaderboard.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
            public IEnumerable<DivisionViewModel> Divisions { get; set; }
            public IEnumerable<LeaderboardViewModel> Recommendations { get; set; }
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
            var user = await _manager.GetCompleteUserAsync(User);
            var userBoardIds = user.UserLeaderboards.Select(ub => ub.Leaderboard.Id);

            // want to exclude the boards that the user is already in
            var recommendations = await _manager.GetRecommendedBoardsQuery(user)
                .Include(b => b.Division)
                .Where(rb => !userBoardIds.Any(ub => ub == rb.Id))
                .ToArrayAsync();

            var divisions = recommendations
                .Select(d => d.Division)
                .Distinct()
                .ToDictionary(d => d.Id);

            var recommendationsByDivision = recommendations
                .GroupBy(rb => rb.DivisionId)
                .ToDictionary(rb => rb.Key, rb => LeaderboardViewModel.Create(rb.AsEnumerable()).ToList());

            var divisionViewModels = recommendationsByDivision.Select(d =>
            {
                return new DivisionViewModel(divisions[d.Key])
                {
                    Boards = d.Value
                };
            });

            return new JsonResult(new ReactState
            {
                User = new UserViewModel(user),
                Recommendations = LeaderboardViewModel.Create(recommendations),
                Divisions = divisionViewModels
            });
        }
    }
}