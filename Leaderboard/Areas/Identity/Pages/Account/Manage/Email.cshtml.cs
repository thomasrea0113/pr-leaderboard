using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Identity.Models;

namespace Leaderboard.Areas.Identity.Pages.Account.Manage
{
    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "New email")]
        public string NewEmail { get; set; }
    }

    public partial class EmailModel : PageModel
    {
        private readonly AppUserManager _userManager;
        private readonly IEmailSender _emailSender;

        public EmailModel(
            AppUserManager userManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        private async Task LoadAsync(ApplicationUser user)
        {
            var email = await _userManager.GetEmailAsync(user).ConfigureAwait(false);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false);
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

        public async Task<IActionResult> OnPostChangeEmailAsync()
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

            var email = await _userManager.GetEmailAsync(user).ConfigureAwait(false);
            if (Input.NewEmail != email)
            {
                if (_userManager.Options.SignIn.RequireConfirmedEmail)
                {
                    var userId = await _userManager.GetUserIdAsync(user).ConfigureAwait(false);
                    var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail).ConfigureAwait(false);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmailChange",
                        pageHandler: null,
                        values: new { userId, email = Input.NewEmail, code },
                        protocol: Request.Scheme);
                    await _emailSender.SendEmailAsync(
                        Input.NewEmail,
                        "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.")
                        .ConfigureAwait(false);

                    StatusMessage = "Confirmation link to change email sent. Please check your email.";
                    return RedirectToPage();
                }
                else
                {
                    user.Email = Input.NewEmail;
                    var result = await _userManager.UpdateAsync(user).ConfigureAwait(false);
                    if (result.Succeeded)
                        StatusMessage = "Your email has been updated.";
                    else
                        StatusMessage = $"Error: {string.Join(", ", result.Errors.Select(e => e.Description))}";
                    return RedirectToPage();
                }
            }

            StatusMessage = "Your email is unchanged.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
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

            var userId = await _userManager.GetUserIdAsync(user).ConfigureAwait(false);
            var email = await _userManager.GetEmailAsync(user).ConfigureAwait(false);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId, code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.")
                .ConfigureAwait(false);

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}
