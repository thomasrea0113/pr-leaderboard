using System;

namespace Leaderboard.Areas.Leaderboards.ViewModels
{
    public enum UploadMethod
    {
        Direct,
        YouTube
    }

    public class SubmitScoreViewModel
    {
        public decimal Score { get; set; }
        public UploadMethod UploadMethod { get; set; }
    }
}
