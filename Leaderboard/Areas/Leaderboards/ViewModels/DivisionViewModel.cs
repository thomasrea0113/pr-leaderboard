using System.Collections.Generic;
using Leaderboard.Areas.Identity.Models;

namespace Leaderboard.Areas.Leaderboards.ViewModels
{
    public class DivisionViewModel
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Slug { get; private set; }
        public List<CategoryViewModel> Categories { get; private set; }
        public GenderValue? Gender { get; private set; }
        public int? AgeLowerBound { get; private set; }
        public int? AgeUpperBound { get; private set; }
    }
}
