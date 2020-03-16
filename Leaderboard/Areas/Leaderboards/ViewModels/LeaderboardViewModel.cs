using System;
using System.Collections.Generic;
using System.Linq;
using Leaderboard.Areas.Leaderboards.Models;
using Microsoft.EntityFrameworkCore;

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
        public string Id { get; private set; }
        public string Name { get; private set; }
        public UnitOfMeasureViewModel UOM { get; private set; }
        public DivisionViewModel Division { get; private set; }
        public WeightClassViewModel WeightClass { get; private set; }

        public LeaderboardViewModel(LeaderboardModel model)
        {
            Id = model.Id;
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

        public static async IAsyncEnumerable<LeaderboardViewModel> FromQueryAsync(IQueryable<LeaderboardModel> query)
        {
            await foreach (var lb in query
                .Include(b => b.Division)
                .Include(b => b.WeightClass)
                .Include(b => b.UOM).ToAsyncEnumerable())
            {
                yield return new LeaderboardViewModel(lb);
            }
        }

        #region equality

        // 2 instances are equal if they have the same Id
#nullable enable
        public override int GetHashCode() => Id.GetHashCode();
        public override bool Equals(object? obj)
        {
            if (obj is LeaderboardModel m)
                return Id.Equals(m.Id);
            return Id.Equals(obj);
        }
#nullable disable

        #endregion
    }
}
