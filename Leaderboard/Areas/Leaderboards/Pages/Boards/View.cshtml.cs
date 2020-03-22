using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Identity.ViewModels;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Areas.Leaderboards.ViewModels;
using Leaderboard.Data;
using Leaderboard.Extensions;
using Leaderboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Areas.Leaderboards.Pages.Boards
{
    public class BoardRouteArgs
    {
        public string Division { get; set; }
        public string Gender { get; set; }
        public string WeightClass { get; set; }
        public string Slug { get; set; }
    }

    public class ViewModel : PageModel
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IMessageQueue _messages;
        private readonly AppUserManager _userManager;

        public LeaderboardModel Board { get; private set; }

        private BoardRouteArgs RouteArgs { get; set; }

        private IQueryable<LeaderboardModel> BoardQuery { get; set; }

        [ViewData]
        public string ModalTitle { get; private set; }

        [ViewData]
        public string ModalBody { get; private set; }

        [ViewData]
        public bool DataModalShow { get; private set; }

        public class ReactProps
        {
            public string ScoresUrl { get; set; }
            public string SubmitScoreUrl { get; set; }
        }
        public ReactProps Props { get; private set; }

        public class ReactState
        {
            public UserViewModel User { get; set; }
            public LeaderboardViewModel Board { get; set; }
            public IEnumerable<ScoreViewModel> Scores { get; set; }
        }

        public ViewModel(ApplicationDbContext ctx, IMessageQueue messages, AppUserManager userManager)
        {
            _ctx = ctx;
            _messages = messages;
            _userManager = userManager;
        }

        private void Init()
        {
            RouteArgs = RouteData.ToObject<BoardRouteArgs>();

            var tinfo = CultureInfo.CurrentCulture.TextInfo;
            var genderValue = Enum.Parse<GenderValues>(tinfo.ToTitleCase(RouteArgs.Gender));

            BoardQuery = _ctx.Leaderboards.AsQueryable().Where(b =>
                b.Division.Slug == RouteArgs.Division && b.Slug == RouteArgs.Slug &&
                (
                    (b.WeightClassId == null && RouteArgs.WeightClass == "any") ||
                    (b.WeightClass.Range == RouteArgs.WeightClass)
                ) &&
                (
                    (b.Division.Gender == null && RouteArgs.Gender == "any") ||
                    (b.Division.Gender == genderValue)
                )
            );

            Props ??= new ReactProps
            {
                ScoresUrl = Url.Page(null, "initial"),
                SubmitScoreUrl = Url.Page(null, "submitScore")
            };
        }

        public void OnGet()
        {
            Init();
        }

        public async Task<JsonResult> OnGetInitialAsync()
        {
            Init();

            var board = await BoardQuery.SingleAsync();
            var scores = await _ctx.Scores.AsQueryable().Where(s => s.BoardId == board.Id).ToArrayAsync();
            var user = await _userManager.GetCompleteUserAsync(User);

            var isMember = await _ctx.Entry(user).Collection(b => b.UserLeaderboards).Query()
                .AnyAsync(ub => ub.LeaderboardId == board.Id);
            var isRecommended = await _userManager.GetRecommendedBoardsQuery(user)
                .AnyAsync(b => b.Id == board.Id);

            return new JsonResult(new ReactState
            {
                User = new UserViewModel(user),
                Board = new UserLeaderboardViewModel(board, isMember, isRecommended),
                Scores = scores.Select(s => new ScoreViewModel(s))
            });
        }

        public async Task OnGetJoinAsync()
        {
            Init();

            // TODO confirm user is not already in this board
            await Task.CompletedTask;

            ModalTitle = "Join Board";
            ModalBody = "Would you like to join this board?";
            DataModalShow = true;
        }

        public async Task<RedirectToPageResult> OnPostModalAsync()
        {
            // TODO implement join
            await Task.CompletedTask;

            _messages.PushMessage("You've joined this board!");
            return RedirectToPage();
        }

        public async Task<RedirectResult> OnPostSubmitScoreAsync()
        {
            var url = Request.ToString();
            return Redirect(url);
        }
    }
}