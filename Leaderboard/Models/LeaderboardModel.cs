using System.Collections.Generic;
using Leaderboard.Models.Relationships;

namespace Leaderboard.Models
{
    // TODO implement sluggy on save
    public class LeaderboardModel
    {
        public string Name { get; set; }
        public List<UserLeaderboard> UserLeaderboards { get; set; }

    }
}