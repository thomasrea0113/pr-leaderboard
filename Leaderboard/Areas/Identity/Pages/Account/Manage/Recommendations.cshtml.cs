using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Identity.ViewModels;
using Leaderboard.Areas.Leaderboards.Models;
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
        private readonly IMapper _mapper;

        public ReactProps Props { get; set; } = new ReactProps();

        private string GetInitialUrl() => Url.Page("", new { handler = "initial" });

        public RecommendationsModel(AppUserManager manager, IMapper mapper)
        {
            _manager = manager;
            _mapper = mapper;
        }

        public void OnGet()
        {
            Props.InitialUrl = GetInitialUrl();
            Props.UserName = User.FindFirst(ClaimTypes.Name).Value;
        }

        private async IAsyncEnumerable<UserLeaderboardViewModel> GetRecommendations(ApplicationUser user)
        {
            var userBoards = _mapper.ProjectTo<UserLeaderboardViewModel>(
                user.UserLeaderboards.Select(ub => ub.Leaderboard).AsQueryable(), new
                {
                    IsMember = true,
                    IsRecommended = false
                });

            var recommendations = await _manager.GetRecommendedBoardsQuery(user)
                .ProjectTo<UserLeaderboardViewModel>(_mapper.ConfigurationProvider, new
                {
                    IsMember = false,
                    IsRecommended = true,
                })
                .ToArrayAsync().ConfigureAwait(false);

            foreach (var board in userBoards.Concat(recommendations).Distinct())
            {
                var model = _mapper.Map<LeaderboardModel>(board);
                board.JoinUrl = model.GetJoinUrl(Url);
                board.ViewUrl = model.GetViewUrl(Url);
                yield return board;
            }
        }

        public async Task<JsonResult> OnGetInitialAsync()
        {
            var user = await _manager.GetCompleteUserAsync(User).ConfigureAwait(false);

            return new JsonResult(new ReactState
            {
                User = _mapper.Map<UserViewModel>(user),
                Recommendations = await GetRecommendations(user).ToArrayAsync().ConfigureAwait(false)
            });
        }
    }
}