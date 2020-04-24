using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Leaderboards.ViewModels;
using Leaderboard.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Areas.Admin.Pages
{
    public class ApproveScore : PageModel
    {
        private readonly ApplicationDbContext _ctx;

        [BindProperty]
        public List<ScoreViewModel> Scores { get; private set; }

        public ApproveScore(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task OnGetAsync()
        {
            var scores = await _ctx.Scores.AsQueryable().Where(s => s.IsApproved == true).ToListAsync();
            Scores = scores.Select(s => new ScoreViewModel(s)).ToList();
        }

        public async Task OnPostAsync()
        {
            await Task.CompletedTask;
        }
    }
}