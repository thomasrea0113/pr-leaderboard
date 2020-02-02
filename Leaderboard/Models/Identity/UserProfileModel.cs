using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Leaderboard.Models.Features;
using Leaderboard.Models.Relationships;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Leaderboard.Models.Identity
{
    public class UserProfileModel : IModelFeatures, IDbActive, IOnDbCreate
    {
    
        public ModelFeatures Features => ModelFeatures.PreventDelete;

        [Key]
        public Guid UserId { get; set; }
        public virtual IdentityUser<Guid> User { get; set; }

        public virtual ICollection<UserLeaderboard> UserLeaderboards { get; set; } = new List<UserLeaderboard>();

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;


        // TODO make sure associated user exists
        public void OnCreate(DbContext ctx, PropertyValues values)
        {
            throw new NotImplementedException();
        }
    }
}