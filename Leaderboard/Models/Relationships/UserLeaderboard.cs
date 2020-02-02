using System;
using System.ComponentModel.DataAnnotations;
using Leaderboard.Models.Features;
using Leaderboard.Models.Identity;

namespace Leaderboard.Models.Relationships
{
    public class UserLeaderboard : AbstractRelationship<UserProfileModel, LeaderboardModel>
    {
        [CompositeKey]
        public Guid UserProfileId { get; set; }
        public virtual UserProfileModel User { get; set; }


        [CompositeKey]
        public Guid LeaderboardId { get; set; }
        public virtual LeaderboardModel Leaderboard { get; set; }
    }
}