using Leaderboard.Areas.Leaderboards.Models;

namespace Leaderboard.Areas.Leaderboards.ViewModels
{
    public class UnitOfMeasureViewModel
    {
        public string Unit { get; set; }

        public UnitOfMeasureViewModel(UnitOfMeasureModel uom)
        {
            Unit = uom.Unit;
        }
    }
}