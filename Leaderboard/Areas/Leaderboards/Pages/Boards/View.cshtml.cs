using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Models;
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
            public LeaderboardViewModel Board { get; set; }
        }
        public ReactProps Props { get; private set; }

        public ViewModel(ApplicationDbContext ctx, IMessageQueue messages)
        {
            _ctx = ctx;
            _messages = messages;
        }

        private async Task InitAsync()
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
                ScoresUrl = Url.Page(null, "scores"),
                Board = await LeaderboardViewModel.FromQueryAsync(BoardQuery).SingleAsync()
            };
        }

        public async Task OnGetAsync()
        {
            await InitAsync();
        }

        public async Task<JsonResult> OnGetScoresAsync()
        {
            await InitAsync();
            var boardId = await BoardQuery.Select(b => b.Id).SingleAsync();
            return new JsonResult(await _ctx.Scores.AsQueryable().Where(s => s.BoardId == boardId).ToArrayAsync());
        }

        public void OnGetJoin()
        {
            // TODO confirm user is not already in this board
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
    }
}