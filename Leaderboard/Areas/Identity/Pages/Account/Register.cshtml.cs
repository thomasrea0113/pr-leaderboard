using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Identity.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Leaderboard.Data;
using Leaderboard.Areas.Leaderboards.Models;
using Microsoft.EntityFrameworkCore;
using Leaderboard.Models.Relationships;
using Leaderboard.Extensions.PageModelExtensions;
using Leaderboard.Services;

namespace Leaderboard.Areas.Identity.Pages.Account
{
    public class RegisterInputModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public List<string> Interests { get; set; }

        public GenderValue? Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Age { get; set; }

        public decimal? Weight { get; set; }
    }

    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppUserManager _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _ctx;
        private readonly IMessageQueue _messages;
        private readonly IPartialRenderer _renderer;

        public RegisterModel(
            AppUserManager userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext ctx,
            IMessageQueue messages,
            IPartialRenderer renderer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _ctx = ctx;
            _messages = messages;
            _renderer = renderer;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; }

        [FromQuery]
        public string ReturnUrl { get; set; }

        [ViewData]
        public string FormIdPrefix { get; set; } = "Register";
        [ViewData]
        public string SubmitButtonText { get; set; } = "Register";

        public SelectList Categories { get; set; }

        /// <summary>
        /// initializes the necessary data from the database for the form
        /// </summary>
        /// <returns></returns>
        private async Task InitModelAsync()
        {
            Categories = new SelectList(await _ctx.Categories
                .AsQueryable()
                .ToArrayAsync().ConfigureAwait(false), nameof(Category.Name), nameof(Category.Name));
        }

        public async Task<IActionResult> OnGetFormAsync()
        {
            await InitModelAsync().ConfigureAwait(false);
            return Partial("Forms/_RegisterFormPartial", this);
        }

        public async Task<IActionResult> OnPostFormAsync()
        {
            ReturnUrl ??= Url.Page("/Account/Manage/Index");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Username,
                    Email = Input.Email,
                    Gender = Input.Gender,
                    Weight = Input.Weight,
                    BirthDate = Input.Age,

                };

                var result = await _userManager.CreateAsync(user, Input.Password).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (Input.Interests != null && Input.Interests.Any())
                    {
                        var categories = await _ctx.Categories.AsQueryable()
                            .Where(c => Input.Interests.Contains(c.Name))
                            .ToListAsync().ConfigureAwait(false);

                        if (categories.Any())
                        {
                            await _ctx.AddRangeAsync(categories.Select(c => new UserCategory
                            {
                                UserId = user.Id,
                                CategoryId = c.Id
                            })).ConfigureAwait(false);
                            await _ctx.SaveChangesAsync().ConfigureAwait(false);
                        }
                    }

                    if (Input.Email != default)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = user.Id, code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            await _renderer.RenderPartialToStringAsync("_EmailConfirmationPartial", user, new { callbackUrl })
                            .ConfigureAwait(false)).ConfigureAwait(false);

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            // TODO if I ever decide to enable required accounts, I should probably push a message
                            return this.JavascriptRedirect(Url.Page("RegisterConfirmation", new { email = Input.Email }));
                        }
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false).ConfigureAwait(false);
                    _messages.PushMessage(await _renderer.RenderPartialToStringAsync("_RegisterSuccessPartial", user)
                        .ConfigureAwait(false));
                    return this.JavascriptRedirect(ReturnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            await InitModelAsync().ConfigureAwait(false);
            return Partial("Forms/_RegisterFormPartial", this);
        }
    }
}
