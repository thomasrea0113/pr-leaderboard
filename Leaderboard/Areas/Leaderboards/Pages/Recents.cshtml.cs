using Leaderboard.Areas.Leaderboards.Controllers;
using Leaderboard.Extensions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;

namespace Leaderboard.Areas.Leaderboards.Pages
{
    public class RecentsReactProps
    {
        public string InitialUrl { get; set; }
        /// <summary>
        /// only retain this number of scores in the recents view
        /// </summary>
        /// <value></value>
        public int Top { get; set; }
        /// <summary>
        /// refresh interval in milliseconds
        /// </summary>
        /// <value></value>
        public int RefreshEvery { get; set; }
    }

    public class RecentsPageModel : PageModel
    {
        private readonly LinkGenerator _link;

        public RecentsPageModel(LinkGenerator link)
        {
            _link = link;
        }

        public RecentsReactProps Props { get; set; }

        private void Init()
        {
            var top = 100;
            Props ??= new RecentsReactProps
            {
                InitialUrl = _link.GetUriByAction(HttpContext,
                    nameof(ScoresController.Get),
                    ControllerExtensions.GetControllerName<ScoresController>(),
                    new ByBoardScoresQuery
                    {
                        IsApproved = true,
                        Top = top
                    }),
                Top = top,
                RefreshEvery = 5000
            };
        }

        public void OnGet() => Init();
    }
}