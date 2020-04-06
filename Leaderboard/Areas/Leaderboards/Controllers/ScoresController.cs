using System;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Areas.Leaderboards.ViewModels;
using Leaderboard.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Areas.Leaderboards.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScoresController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;
        private readonly AppUserManager _um;

        public ScoresController(ApplicationDbContext ctx, AppUserManager userManager)
        {
            _ctx = ctx;
            _um = userManager;
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<SubmitScoreViewModel>> Create(SubmitScoreViewModel model)
        {
            if (ModelState.IsValid)
            {
                var boardId = await _ctx.Set<LeaderboardModel>().AsQueryable()
                    .Where(b => b.Slug == model.BoardSlug)
                    .Select(b => b.Id)
                    .SingleAsync();
                var score = _ctx.Set<ScoreModel>().Add(new ScoreModel
                {
                    IsApproved = false,
                    BoardId = boardId,
                    UserId = _um.GetUserId(User),
                    Value = model.Score
                }).Entity;
                return Created(Url.Page("Boards/View", null, null, null, null, $"score={score.Id}"), score);
            }
            else
                return BadRequest(ModelState);
        }
    }
}