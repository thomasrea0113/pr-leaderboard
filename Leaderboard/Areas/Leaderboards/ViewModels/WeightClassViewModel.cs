using Leaderboard.Areas.Leaderboards.Models;

namespace Leaderboard.Areas.Leaderboards.ViewModels
{
    public class WeightClassViewModel
    {
        public int? WeightLowerBound { get; set; }
        public int? WeightUpperBound { get; set; }
        public WeightClassViewModel(WeightClass weightClass)
        {
            WeightLowerBound = weightClass.WeightLowerBound;
            WeightUpperBound = weightClass.WeightUpperBound;
        }
    }
}
