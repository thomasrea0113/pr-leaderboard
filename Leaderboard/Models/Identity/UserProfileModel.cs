using System;
using System.Collections.Generic;
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

        public Guid UserId { get; set; }
        public IdentityUser User { get; set; }

        public List<UserLeaderboard> UserLeaderboards { get; set; }

        public bool IsActive { get; set; }

        // TODO make sure associated user exists
        public void OnCreate(DbContext ctx, PropertyValues values)
        {
            throw new NotImplementedException();
        }
    }
}