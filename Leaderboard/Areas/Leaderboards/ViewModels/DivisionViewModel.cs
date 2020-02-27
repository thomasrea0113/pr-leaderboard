using System;
using System.Collections.Generic;
using System.Linq;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Leaderboards.Models;

namespace Leaderboard.Areas.Leaderboards.ViewModels
{
    public class DivisionViewModel
    {
        public string Name { get; set; }
        public GenderValues? Gender { get; set; }
        public int? AgeLowerBound { get; set; }
        public int? AgeUpperBound { get; set; }

        public DivisionViewModel(Division division)
        {
            Name = division.Name;
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
