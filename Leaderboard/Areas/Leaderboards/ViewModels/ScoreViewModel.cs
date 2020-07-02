using System;
using Leaderboard.Areas.Identity.ViewModels;

namespace Leaderboard.Areas.Leaderboards.ViewModels
{
    public class ScoreViewModel
    {
        public string Id { get; set; }
        public bool IsApproved { get; set; }
        public LeaderboardViewModel Board { get; set; }
        public UserViewModel User { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Value { get; set; }
    }
}
