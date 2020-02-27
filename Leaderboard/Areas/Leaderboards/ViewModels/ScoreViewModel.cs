using System;
using Leaderboard.Areas.Leaderboards.Models;

namespace Leaderboard.Areas.Leaderboards.ViewModels
{
    public class ScoreViewModel
    {
        public bool IsApproved { get; set; }
        public string LeaderboardId { get; set; }
        public string UserId { get; set; }
        public decimal Value { get; set; }

        public ScoreViewModel(ScoreModel score)
        {
            IsApproved = Convert.ToBoolean(score.IsApproved);
            LeaderboardId = score.BoardId;
            UserId = score.UserId;
            Value = score.Value;
        }
    }
}
