using System.Collections.Generic;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Leaderboard.ViewModels;
using Leaderboard.Areas.Leaderboards.Models;
using Microsoft.Extensions.Options;
using System;

namespace Leaderboard.Pages
{
    public class Index : PageModel
    {
        private readonly AppUserManager _manager;
        private readonly string[] _bgUrl;

        public class ReactProps
        {
            public string InitialUrl { get; set; }

            /// <summary>
            /// Any of array of stacked background images, from top to bottom
            /// </summary>
            /// <value></value>
            public string[] BackgroundImages { get; set; }
        }

        /// <summary>
        /// data to be sent to the react component on initial load
        /// </summary>
        public class ReactData
        {
            public IEnumerable<FeaturedViewModel> Featured { get; set; }
        }

        [BindProperty]
        public ReactProps Props { get; set; }

        public Index(AppUserManager manager, IOptionsSnapshot<AppConfiguration> config)
        {
            _manager = manager;
            _bgUrl = config.Value.HomeBackgroundUrls;
        }

        public void Initialize()
        {
            Props ??= new ReactProps
            {
                InitialUrl = Url.Page(null, "initial"),
                BackgroundImages = _bgUrl ?? Array.Empty<string>()
            };
        }

        private IEnumerable<FeaturedViewModel> ToFeatureViewModel(IEnumerable<ScoreModel> models)
        {
            foreach (var model in models)
            {
                var args = model.Board.GetViewArgs();
                yield return new FeaturedViewModel
                {
                    Title = $"{model.Board.Name} in the {model.Board.Division.Name}",
                    Description = $"user {model.User.UserName}",
                    Links = new[] {
                            new Link {
                                Label = "View User",
                                Url = Url.Page(null), // TODO set link
                                ClassName = BootstrapColorClass.Warning,
                                Addon = FontawesomeIcon.User
                            },
                            new Link {
                                Label = "View Board",
                                Url = Url.Page("/Boards/View", args),
                                ClassName = BootstrapColorClass.Success,
                                Addon = FontawesomeIcon.Go
                            }
                        }
                };
            }
        }

        public async Task<JsonResult> OnGetInitial()
        {
            var featured = await _manager.GetFeatured()
                .Include(f => f.Board)
                    .ThenInclude(f => f.Division)
                .Include(f => f.Board)
                    .ThenInclude(f => f.WeightClass)
                .Include(f => f.User)
                .Take(3).ToArrayAsync();

            return new JsonResult(new ReactData
            {
                Featured = ToFeatureViewModel(featured)
            });
        }

        public void OnGet()
        {
            Initialize();
        }

        public async Task OnPostAsync()
        {
            await Task.CompletedTask;
        }
    }
}