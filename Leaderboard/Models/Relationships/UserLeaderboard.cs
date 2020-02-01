using System;
using System.ComponentModel.DataAnnotations;
using Leaderboard.Models.Features;
using Leaderboard.Models.Identity;

namespace Leaderboard.Models.Relationships
{
    public class UserLeaderboard : AbstractRelationship<UserProfileModel, LeaderboardModel>
    {
        [CompositeKey]
        public Guid UserId { get; set; }
        public UserProfileModel User { get; set; }


        [CompositeKey]
        public string LeaderboardId { get; set; }
        public LeaderboardModel Leaderboard { get; set; }
    }
}