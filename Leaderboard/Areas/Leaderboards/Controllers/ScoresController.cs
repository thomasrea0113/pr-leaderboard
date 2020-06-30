using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Areas.Leaderboards.ViewModels;
using Leaderboard.Data;
using Leaderboard.Extensions;
using Leaderboard.Models.Relationships;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Areas.Leaderboards.Controllers
{
    public class ApproveModel
    {
        public IList<string> Ids { get; set; }
    }

    public class ScoresQuery
    {
        public bool? IsApproved { get; set; }

        public int? Top { get; set; }
    }

    public class ByBoardScoresQuery : ScoresQuery
    {
        public int? TopBoards { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ScoresController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;
        private readonly AppUserManager _um;
        private readonly IMapper _mapper;

        public ScoresController(ApplicationDbContext ctx, AppUserManager userManager, IMapper mapper)
        {
            _ctx = ctx;
            _um = userManager;
            _mapper = mapper;
        }

        [Authorize(Policy = "AppAdmin")]
        [HttpPatch]
        [Route("[action]")]
        public async IAsyncEnumerable<ScoreModel> Approve(ApproveModel m)
        {
            var ids = m.Ids;
            var scores = _ctx.Scores.AsQueryable()
                .Where(s => !s.IsApproved)
                .Where(s => ids.Contains(s.Id))
                .AsAsyncEnumerable();

            await foreach (var score in scores)
            {
                score.IsApproved = true;
                yield return score;
            }

            await _ctx.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Get's all scores
        /// </summary>
        /// <param name="isApproved">indicates whether to return scores that are only approved/not approved. Null indicates all scores</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IQueryable<ScoreViewModel> Get([FromQuery] ScoresQuery filter = null)
        {
            var isApproved = filter?.IsApproved;
            var top = filter?.Top;

            var query = _ctx.Scores.AsQueryable();

            if (isApproved != true && !User.IsInRole("Admin"))
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return null;
            }
            else
            {

                if (isApproved != null)
                    query = query.Where(s => s.IsApproved == isApproved);

                if (top != null)
                    query = query.Take((int)top);

                return _mapper.ProjectTo<ScoreViewModel>(query);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IDictionary<string, IEnumerable<ScoreViewModel>>> ByBoard([FromQuery] ByBoardScoresQuery filter = null)
        {
            var top = filter?.Top;
            var topBoards = filter?.TopBoards;

            // if we're selecting only x top boards, then 'top' referring to the top count from each board.
            // so we have to apply the top filter after we've performed the grouping
            if (topBoards != null)
                filter.Top = null;

            // EF Core doesn't support groupby, so we have to evaluate the query and do it in memory
            var scores = await Get(filter).ToArrayAsync().ConfigureAwait(false);

            var scoresQuery = scores.GroupBy(s => s.BoardId);

            if (topBoards != null)
                scoresQuery = scoresQuery.Take((int)filter.TopBoards);

            if (top != null)
                scoresQuery = scoresQuery.SelectMany(g => g.Take((int)top).GroupBy(s => s.BoardId));



            return scoresQuery.ToDictionary(g => g.Key, g => g.AsEnumerable());
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<SubmitScoreViewModel>> Create(SubmitScoreViewModel model)
        {
            // because this is an ApiController, the model state is automatically verified before
            // the action is called. We don't have to explicitly check it here.
            // TODO how to add custom server-side validation here?
            var board = await _ctx.Set<LeaderboardModel>().AsQueryable()
                .Where(b => b.Id == model.BoardId)
                .Include(b => b.Division)
                .Include(b => b.WeightClass)
                .SingleAsync().ConfigureAwait(false);

            var isMember = (await _ctx.Set<UserLeaderboard>().AsQueryable()
                .CountAsync(ub => ub.UserId == _um.GetUserId(User) && ub.LeaderboardId == board.Id)
                .ConfigureAwait(false)
            ) == 1;

            if (!isMember)
            {
                ModelState.AddModelError(nameof(SubmitScoreViewModel.UserName), $"You are not a member of board '{board.Slug}'");
                return this.ValidationError();
            };

            var score = _ctx.Set<ScoreModel>().Add(new ScoreModel
            {
                IsApproved = false,
                BoardId = board.Id,
                UserId = _um.GetUserId(User),
                Value = model.Score
            }).Entity;

            await _ctx.SaveChangesAsync().ConfigureAwait(false);

            var page = Url.Page("/Boards/View", null, board.GetViewArgs(), null, null, $"score={score.Id}");

            // TODO generate board slug
            return Created(page, score);
        }
    }
}
