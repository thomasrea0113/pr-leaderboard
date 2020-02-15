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
        private readonly AppUserManager _manager;

        [BindProperty]
        public bool EnableEmail { get; set; } = true;

        [BindProperty]
        public ContactViewModel ContactModel { get; set; }

        [ViewData]
        public string FormIdPrefix { get; set; } = "Contact";

        public Contact(AppUserManager manager)
        {
            _manager = manager;
        }

        public void OnGet()
        {
            ContactModel = new ContactViewModel();
        }

        public async Task<IActionResult> OnGetFormAsync()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                ContactModel ??= new ContactViewModel();
                var user = await _manager.GetUserAsync(HttpContext.User);
                ContactModel.Email ??= user?.Email;
                EnableEmail = false;
            }
            return Partial("Forms/_ContactFormPartial", ContactModel);
        }

        public IActionResult OnPostFormAsync()
        {
            if (ModelState.IsValid)
            {

            }

            ModelState.AddModelError("Email", "Well well well!");

            return Partial("Forms/_ContactFormPartial", ContactModel);
        }
    }
}