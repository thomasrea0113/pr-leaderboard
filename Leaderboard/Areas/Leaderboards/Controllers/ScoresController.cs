using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [ApiController]
    [Route("api/[controller]")]
    public class ScoresController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;
        private readonly AppUserManager _um;

        public ScoresController(ApplicationDbContext ctx, AppUserManager userManager)
        {
            _ctx = ctx;
            _um = userManager;
        }

        [Authorize(Roles = "admin")]
        [HttpPatch]
        [Route("[action]")]
        public async Task Approve(string[] Ids)
        {
            var scores = _ctx.Scores.AsQueryable()
                .Where(s => !s.IsApproved)
                .Where(s => Ids.Contains(s.Id));

            foreach (var score in scores)
                score.IsApproved = true;

            await _ctx.SaveChangesAsync();
        }

        /// <summary>
        /// Get's all scores
        /// </summary>
        /// <param name="isApproved">indicates whether to return scores that are only approved/not approved. Null indicates all scores</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<IEnumerable<ScoreViewModel>> All(bool? isApproved = null)
        {
            var query = _ctx.Scores.AsQueryable();

            if (isApproved != null)
                query = query.Where(s => s.IsApproved == isApproved);

            return (await query.ToArrayAsync()).Select(s => new ScoreViewModel(s));
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
                .SingleAsync();

            var isMember = (await _ctx.Set<UserLeaderboard>().AsQueryable()
                .CountAsync(ub => ub.UserId == _um.GetUserId(User) && ub.LeaderboardId == board.Id)
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

            await _ctx.SaveChangesAsync();

            var page = Url.Page("/Boards/View", null, board.GetViewArgs(), null, null, $"score={score.Id}");

            // TODO generate board slug
            return Created(page, score);
        }
    }
}
