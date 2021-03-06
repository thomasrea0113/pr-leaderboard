using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Identity.ViewModels;
using Leaderboard.Areas.Leaderboards.Controllers;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Areas.Leaderboards.ViewModels;
using Leaderboard.Data;
using Leaderboard.Extensions;
using Leaderboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
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

    public class ReactProps
    {
        public string ScoresUrl { get; set; }
        public string SubmitScoreUrl { get; set; }
        public ClientFormFieldAttribuleMap<SubmitScoreViewModel> FieldAttributes { get; set; }
    }

    public class ReactState
    {
        public UserViewModel User { get; set; }
        public LeaderboardViewModel Board { get; set; }
        public IEnumerable<ScoreViewModel> Scores { get; set; }
    }

    public class ViewModel : PageModel
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IMessageQueue _messages;
        private readonly AppUserManager _userManager;
        private readonly IFormFieldAttributeProvider _formFieldAttributeProvider;
        private readonly ScoresController _scoresController;
        private readonly IMapper _mapper;

        public ReactProps Props { get; private set; }
        public LeaderboardModel Board { get; private set; }
        private BoardRouteArgs RouteArgs { get; set; }
        private IQueryable<LeaderboardModel> BoardQuery { get; set; }

        [ViewData]
        public string ModalTitle { get; private set; }

        [ViewData]
        public string ModalBody { get; private set; }

        [ViewData]
        public bool DataModalShow { get; private set; }

        public ViewModel(
            ApplicationDbContext ctx,
            IMessageQueue messages,
            AppUserManager userManager,
            IFormFieldAttributeProvider formFieldAttributeProvider,
            ScoresController scoresController,
            IMapper mapper)
        {
            _ctx = ctx;
            _messages = messages;
            _userManager = userManager;
            _formFieldAttributeProvider = formFieldAttributeProvider;
            _scoresController = scoresController;
            _mapper = mapper;
        }

        private void Init()
        {
            RouteArgs = RouteData.ToObject<BoardRouteArgs>();

            var tinfo = CultureInfo.CurrentCulture.TextInfo;
            var genderValue = Enum.Parse<GenderValue>(tinfo.ToTitleCase(RouteArgs.Gender));

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
            )
                .Include(b => b.Division)
                .Include(b => b.UOM);

            Props ??= new ReactProps
            {
                ScoresUrl = Url.Page(null, "initial"),
                SubmitScoreUrl = Url.Action("create", "scores"),
                FieldAttributes = _formFieldAttributeProvider.GetFieldAttriutesForModel<SubmitScoreViewModel>(true)
            };
        }

        public void OnGet()
        {
            Init();
        }

        public async Task<JsonResult> OnGetInitialAsync()
        {
            Init();

            var board = await BoardQuery.SingleAsync().ConfigureAwait(false);

            var userBoardViewModel = _mapper.Map<UserLeaderboardViewModel>(board);
            userBoardViewModel.IsMember = false;
            userBoardViewModel.IsRecommended = false;
            userBoardViewModel.ViewUrl = Url.Page("/Boards/View", board.ViewArgs);
            userBoardViewModel.JoinUrl = Url.Page("/Boards/View", "join", board.ViewArgs);

            var scores = _scoresController.GetScores(new ScoresQuery
            {
                IsApproved = true,
                BoardId = board.Id,
            });

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetCompleteUserAsync(User).ConfigureAwait(false);
                var userViewModel = _mapper.Map<UserViewModel>(user);

                userBoardViewModel.IsMember = await _ctx.Entry(user).Collection(b => b.UserLeaderboards).Query()
                    .AnyAsync(ub => ub.LeaderboardId == board.Id).ConfigureAwait(false);
                userBoardViewModel.IsRecommended = await _userManager.GetRecommendedBoardsQuery(user)
                    .AnyAsync(b => b.Id == board.Id).ConfigureAwait(false);

                return new JsonResult(new ReactState
                {
                    User = userViewModel,
                    Board = userBoardViewModel,
                    Scores = scores,
                });
            }

            return new JsonResult(new ReactState
            {
                Board = userBoardViewModel,
                Scores = scores,
            });
        }

        public Task OnGetJoin()
        {
            Init();

            // TODO confirm user is not already in this board

            ModalTitle = "Join Board";
            ModalBody = "Would you like to join this board?";
            DataModalShow = true;
            return Task.CompletedTask;
        }

        public Task<RedirectToPageResult> OnPostModalAsync()
        {

            _messages.PushMessage("You've joined this board!");
            return Task.FromResult(RedirectToPage());
        }
    }
}