using Leaderboard.Areas.Leaderboards.Models;

namespace Leaderboard.Areas.Leaderboards.ViewModels
{
    public class ScoreViewModel
    {
        public string Id { get; set; }
        public bool IsApproved { get; set; }
        public string BoardId { get; set; }
        public string UserId { get; set; }
        public decimal Value { get; set; }

        public ScoreViewModel(ScoreModel score)
        {
            Id = score.Id;
            IsApproved = score.IsApproved;
            BoardId = score.BoardId;
            UserId = score.UserId;
            Value = score.Value;
        }
    }
}
