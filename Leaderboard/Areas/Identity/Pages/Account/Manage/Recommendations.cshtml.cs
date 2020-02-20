using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Identity.ViewModels;
using Leaderboard.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            => new JsonResult(new ReactState
        {
            User = (await _manager.GetUserAsync(User)).ToObject<UserViewModel>()
        });
    }
}