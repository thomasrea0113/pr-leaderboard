using System.Collections.Generic;
using System.Threading.Tasks;
using Leaderboard.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Leaderboard.Pages.Shared
{
    public abstract class LayoutModel : PageModel
    {
        public virtual IList<Link> AdminLinks { get; }

        public LayoutModel()
        {

        }
    }
}