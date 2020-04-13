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
        public string BoardId { get; set; }

        public decimal Score { get; set; }
    }
}
