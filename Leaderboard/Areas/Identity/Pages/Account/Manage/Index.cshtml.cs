using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Data;
using Leaderboard.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Areas.Identity.Pages.Account.Manage
{
    public class IndexInputModel
    {
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Active Leaderboards")]
        public ICollection<LeaderboardModel> Leaderboards { get; set; }
    }

    public partial class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _ctx;
        private readonly AppUserManager _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            ApplicationDbContext ctx,
            AppUserManager userManager,
            SignInManager<ApplicationUser> signInManager,
            IMessageQueue messages)
        {
            _ctx = ctx;
            _userManager = userManager;
            _signInManager = signInManager;
            Messages = messages;
        }

        public string Username { get; set; }

        public ICollection<LeaderboardModel> Leaderboards { get; set; }

        [BindProperty]
        public IndexInputModel Input { get; set; }

        public IMessageQueue Messages { get; set; }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user).ConfigureAwait(false);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user).ConfigureAwait(false);

            Username = userName;

            Leaderboards = await _ctx.UserLeaderboards.AsQueryable()
                .Where(ub => ub.UserId == user.Id)
                .Select(ub => ub.Leaderboard)
                .ToListAsync().ConfigureAwait(false);

            Input = new IndexInputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user).ConfigureAwait(false);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user).ConfigureAwait(false);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user).ConfigureAwait(false);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber).ConfigureAwait(false);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user).ConfigureAwait(false);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            await _signInManager.RefreshSignInAsync(user).ConfigureAwait(false);
            Messages.PushMessage("Your profile has been updated");
            return RedirectToPage();
        }
    }
}
