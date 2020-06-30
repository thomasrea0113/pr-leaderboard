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
        public string Id { get; private set; }
        public string Name { get; private set; }
        public UnitOfMeasureViewModel UOM { get; private set; }
        public DivisionViewModel Division { get; private set; }
        public WeightClassViewModel WeightClass { get; private set; }
        public string Slug { get; private set; }

        public string ViewUrl { get; set; }
        public string JoinUrl { get; set; }

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
