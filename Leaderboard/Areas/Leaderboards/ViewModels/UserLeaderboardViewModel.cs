using System.Collections.Generic;
using Leaderboard.Areas.Leaderboards.Models;
using Microsoft.AspNetCore.Mvc;

namespace Leaderboard.Areas.Leaderboards.ViewModels
{
    /// <summary>
    /// An extension of the Leaderboard view model, that also includes information relavent to a specific user
    /// </summary>
    public class UserLeaderboardViewModel : LeaderboardViewModel
    {
        public bool IsMember { get; private set; }
        public bool IsRecommended { get; private set; }
        public string ViewUrl { get; set; }
        public string JoinUrl { get; set; }

        public UserLeaderboardViewModel(
            LeaderboardModel model,
            bool isMember,
            bool isRecommended,
            IUrlHelper url = default) : base(model)
        {
            IsMember = isMember;
            IsRecommended = isRecommended;

            if (url != default)
            {
                var args = new
                {
                    area = "Leaderboards",
                    division = model.Division.Slug,
                    gender = model.Division.Gender?.ToString().ToLower() ?? "any",
                    weightClass = model.WeightClass?.Range ?? "any",
                    slug = model.Slug
                };
                // NOTE - page will return null if it does not resolve to a valid url (including any route constraints)
                ViewUrl = url.Page("/Boards/View", args);
                JoinUrl = url.Page("/Boards/View", "join", args);
            }
        }

        public static IEnumerable<UserLeaderboardViewModel> Create(
            IEnumerable<LeaderboardModel> models,
            bool isMember,
            bool isRecommended,
            IUrlHelper url = default)
        {
            foreach (var model in models)
                yield return new UserLeaderboardViewModel(model, isMember, isRecommended, url);
        }

        #region equality

        // 2 instances are equal if they have the same Id
#nullable enable
        public override int GetHashCode() => Id.GetHashCode();
        public override bool Equals(object? obj)
        {
            if (obj is LeaderboardModel m)
                return Id.Equals(m.Id);
            return Id.Equals(obj);
        }
#nullable disable

        #endregion
    }
}
