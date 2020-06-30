using Leaderboard.Areas.Leaderboards.Models;

namespace Leaderboard.Areas.Leaderboards.ViewModels
{
    /// <summary>
    /// An extension of the Leaderboard view model, that also includes information relavent to a specific user
    /// </summary>
    public class UserLeaderboardViewModel : LeaderboardViewModel
    {
        public bool IsMember { get; set; }
        public bool IsRecommended { get; set; }

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
