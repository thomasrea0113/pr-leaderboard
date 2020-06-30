using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Leaderboard.Areas.Leaderboards.Pages
{
    public class RecentsReactProps
    {
        public string InitialUrl { get; set; }
    }

    public class RecentsPageModel : PageModel
    {
        public RecentsReactProps Props { get; set; }

        public RecentsPageModel()
        {
        }

        private void Init()
        {
            Props ??= new RecentsReactProps
            {
                InitialUrl = Url.Link(null, new { handler = "initial" })
            };
        }

        public void OnGet() => Init();
    }
}