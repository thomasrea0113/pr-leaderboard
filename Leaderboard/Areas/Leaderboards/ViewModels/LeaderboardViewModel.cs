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
        public LeaderboardModel Board { get; private set; }
        public DivisionViewModel Division { get; private set; }
        public WeightClass WeightClass { get; private set; }
        public LeaderboardViewModel(LeaderboardModel model)
        {
            Board = model;
            Division = new DivisionViewModel(model.Division);
            WeightClass = model.WeightClass;
        }

        public static IEnumerable<LeaderboardViewModel> Create(params LeaderboardModel[] models)
        {
            foreach (var model in models)
                yield return new LeaderboardViewModel(model);
        }
    }
}
