using System;
using System.Collections.Generic;
using System.Linq;
using Leaderboard.Areas.Leaderboards.Models;

namespace Leaderboard.Areas.Leaderboards.ViewModels
{
    /// <summary>
    /// This view model is important, even though is simply exposes the Division/WeightClass
    /// because System.Text.Json does not support cyclic references, so navigation properties
    /// are not serialized. This means that if we simply serialized the Model, it would
    /// only have the DivisionId and WeightClassId, but no useful information.
    /// </summary>
    public class LeaderboardViewModel
    {
        public string Name { get; set; }
        public UnitOfMeasureViewModel UOM { get; set; }
        public DivisionViewModel Division { get; set; }
        public WeightClassViewModel WeightClass { get; set; }

        public LeaderboardViewModel(LeaderboardModel model)
        {
            Name = model.Name;
            Division = new DivisionViewModel(model.Division);
            UOM = new UnitOfMeasureViewModel(model.UOM);
            if (model.WeightClass != null)
                WeightClass = new WeightClassViewModel(model.WeightClass);
        }

        public static IEnumerable<LeaderboardViewModel> Create(IEnumerable<LeaderboardModel> models)
        {
            foreach (var model in models)
                yield return new LeaderboardViewModel(model);
        }
    }
}
