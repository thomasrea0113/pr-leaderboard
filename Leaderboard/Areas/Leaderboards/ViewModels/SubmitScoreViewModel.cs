using System;
using System.ComponentModel.DataAnnotations;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Areas.Leaderboards.Validators;

namespace Leaderboard.Areas.Leaderboards.ViewModels
{
    public class SubmitScoreViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [ValidSlug(typeof(LeaderboardModel), "'{0}' is not a valid slug for an existing leaderboard")]
        public string BoardSlug { get; set; }

        [Required]
        public decimal Score { get; set; }
    }
}
