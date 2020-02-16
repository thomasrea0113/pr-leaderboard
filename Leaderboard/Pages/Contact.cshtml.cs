using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Leaderboard.Pages
{
    public class Contact : PageModel
    {
        [ViewData]
        public bool EmailDisabled => HttpContext.User.Claims
            .Any(c => c.Type == ClaimTypes.Email && c.Value != default);

        public string Email => HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

        [BindProperty]
        public ContactViewModel ContactModel { get; set; }

        public IActionResult OnGetForm()
        {
            ContactModel ??= new ContactViewModel();
            ContactModel.Email = Email;
            return Partial("Forms/_ContactFormPartial", ContactModel);
        }

        public async Task<IActionResult> OnPostFormAsync()
        {
            if (ModelState.IsValid)
            {
                // TODO implement email
                await Task.CompletedTask;
            }

            ContactModel ??= new ContactViewModel();
            ContactModel.Email = Email;
            return Partial("Forms/_ContactFormPartial", ContactModel);
        }
    }
}