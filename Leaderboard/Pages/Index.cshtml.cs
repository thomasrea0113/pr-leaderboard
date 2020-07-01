using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Leaderboard.ViewModels;
using Microsoft.Extensions.Options;
using System;

namespace Leaderboard.Pages
{
    /// <summary>
    /// data to be sent to the react component on initial load
    /// </summary>
    public class ReactData
    {
        public IEnumerable<FeaturedViewModel> Featured { get; set; }
    }

    public class ReactProps
    {
        public string InitialUrl { get; set; }

        /// <summary>
        /// Any of array of stacked background images, from top to bottom
        /// </summary>
        /// <value></value>
        public IList<string> BackgroundImages { get; set; }
    }

    public class Index : PageModel
    {
        private readonly IList<string> _bgUrl;

        [BindProperty]
        public ReactProps Props { get; set; }

        public Index(IOptionsSnapshot<AppConfiguration> config)
        {
            _bgUrl = config.Value.HomeBackgroundUrls;
        }

        public void Initialize()
        {
            Props ??= new ReactProps
            {
                BackgroundImages = _bgUrl ?? Array.Empty<string>()
            };
        }

        public void OnGet()
        {
            Initialize();
        }
    }
}