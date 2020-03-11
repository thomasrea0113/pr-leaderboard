using System;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Data;
using Leaderboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Areas.Leaderboards.Pages.Boards
{
    public class ViewModel : PageModel
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IMessageQueue _messages;

        public LeaderboardModel Board { get; private set; }

        [ViewData]
        public string ModalTitle { get; private set; }

        [ViewData]
        public string ModalBody { get; private set; }

        [ViewData]
        public bool DataModalShow { get; private set; }

        public ViewModel(ApplicationDbContext ctx, IMessageQueue messages)
        {
            _ctx = ctx;
            _messages = messages;
        }

        private async Task InitAsync()
        {
            var division = RouteData.Values["division"].ToString();
            var gender = RouteData.Values["gender"].ToString();
            var weightClass = RouteData.Values["weightClass"].ToString();
            var slug = RouteData.Values["slug"].ToString();

            var genderValue = Enum.Parse<GenderValues>(gender, true);

            var query = _ctx.Leaderboards.AsQueryable().Where(b =>
                b.Division.Slug == division && b.Slug == slug &&
                (
                    (b.WeightClassId == null && weightClass == "any") ||
                    (b.WeightClass.Range == weightClass)
                ) &&
                (
                    (b.Division.Gender == null && gender == "any") ||
                    (b.Division.Gender == genderValue)
                )
            );

            Board = await query.SingleAsync();
        }

        public async Task OnGetAsync()
        {
            await InitAsync();
        }

        public async Task OnGetJoinAsync()
        {
            await InitAsync();
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