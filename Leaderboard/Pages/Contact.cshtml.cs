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

        [ViewData]
        public string FormIdPrefix => "Contact";

        public string Email => HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

        [BindProperty]
        public ContactViewModel ContactModel { get; set; }

        public void OnGet()
        {
            ContactModel ??= new ContactViewModel();
            ContactModel.Email = Email;
        }

        public async Task<IActionResult> OnPostFormAsync()
        {
            if (ModelState.IsValid)
            {
                // make sure the user didn't change the email on the client.
                ContactModel.Email = Email ?? ContactModel.Email;

                // TODO implement email
                await Task.CompletedTask;
            }
            return Partial("Forms/_ContactFormPartial", ContactModel);
        }
    }
}