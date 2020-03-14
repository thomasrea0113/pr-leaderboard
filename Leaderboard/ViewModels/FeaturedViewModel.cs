using System;
using System.Collections.Generic;

namespace Leaderboard.ViewModels
{
    public class FeaturedViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public IEnumerable<Link> Links { get; set; }
    }
}
