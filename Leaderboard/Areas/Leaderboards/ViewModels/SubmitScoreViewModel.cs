using System.ComponentModel.DataAnnotations;

namespace Leaderboard.Areas.Leaderboards.ViewModels
{
    public class SubmitScoreViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string BoardId { get; set; }

        public decimal Score { get; set; }
    }
}
