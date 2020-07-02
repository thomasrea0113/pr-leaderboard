using Leaderboard.Areas.Leaderboards.Controllers;
using Leaderboard.Extensions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;

namespace Leaderboard.Areas.Leaderboards.Pages
{
    public class RecentsReactProps
    {
        public string InitialUrl { get; set; }
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
            Props ??= new RecentsReactProps
            {
                InitialUrl = _link.GetUriByAction(HttpContext,
                    nameof(ScoresController.Get),
                    ControllerExtensions.GetControllerName<ScoresController>(),
                    new ByBoardScoresQuery
                    {
                        IsApproved = true,
                        Top = 100
                    })
            };
        }

        public void OnGet() => Init();
    }
}