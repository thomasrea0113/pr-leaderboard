using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Leaderboard.Areas.Leaderboards.Pages.Boards
{
    public class ViewModel : PageModel
    {
        public ViewModel()
        {

        }

        public async Task OnGetAsync()
        {
            await Task.CompletedTask;
        }

        public async Task OnPostAsync()
        {
            await Task.CompletedTask;
        }
    }
}