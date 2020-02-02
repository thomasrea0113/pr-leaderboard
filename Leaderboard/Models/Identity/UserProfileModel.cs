using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Leaderboard.Models.Features;
using Leaderboard.Models.Relationships;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Leaderboard.Models.Identity
{
    public class UserProfileModel : IModelFeatures, IDbActive
    {
        public ModelFeatures Features => ModelFeatures.PreventDelete;

        [Key]
        public Guid UserId { get; set; }
        public virtual IdentityUser<Guid> User { get; set; }

        public virtual ICollection<UserLeaderboard> UserLeaderboards { get; set; } = new List<UserLeaderboard>();

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        public UserProfileModel()
        {
        }
    }
}