using System;
using Leaderboard.Models.Identity;

namespace Leaderboard.Models.Relationships
{
    public class UserLeaderboard : AbstractRelationship<UserModel, LeaderboardModel>
    {
        public Guid UserId { get; set; }
        public UserModel User { get; set; }

        public string LeaderboardId { get; set; }
        public LeaderboardModel Leaderboard { get; set; }
    }
}