using System;
using System.Collections.Generic;
using System.Linq;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Leaderboards.Models;

namespace Leaderboard.Areas.Leaderboards.ViewModels
{
    public class DivisionViewModel
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public List<CategoryViewModel> Categories { get; private set; }
        public GenderValues? Gender { get; private set; }
        public int? AgeLowerBound { get; private set; }
        public int? AgeUpperBound { get; private set; }

        public DivisionViewModel(Division division)
        {
            Id = division.Id;
            Name = division.Name;

            if (division.DivisionCategories?.Any() == true)
                Categories = CategoryViewModel.Create(division.DivisionCategories.Select(dc => dc.Category)).ToList();

            Gender = division.Gender;
            AgeLowerBound = division.AgeLowerBound;
            AgeUpperBound = division.AgeUpperBound;
        }

        public static IEnumerable<DivisionViewModel> Create(params Division[] divisions)
        {
            foreach (var division in divisions)
                yield return new DivisionViewModel(division);
        }
    }
}
