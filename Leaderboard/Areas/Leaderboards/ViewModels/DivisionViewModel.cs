using System;
using System.Collections.Generic;
using System.Linq;
using Leaderboard.Areas.Leaderboards.Models;

namespace Leaderboard.Areas.Leaderboards.ViewModels
{
    public class DivisionViewModel
    {
        public Division Division { get; private set; }
        public List<Category> Categories { get; private set; }

        public DivisionViewModel(Division model)
        {
            Division = model;
            Categories = model.DivisionCategories.Select(dc => dc.Category).ToList();
        }
    }
}
