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
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Models.Identity
{
    public class UserProfileModel : IModelFeatures, IDbActive, IDbEntity<UserProfileModel>
    {
        public ModelFeatures Features => ModelFeatures.PreventDelete;

        [Key]
        public Guid UserId { get; set; }

        // This can't be lazy loaded, because the IdentityUser model doesn't have a Profile property
        public virtual IdentityUser<Guid> User { get; set; }

        public virtual ICollection<UserLeaderboard> UserLeaderboards { get; set; } = new List<UserLeaderboard>();

        public bool? IsActive { get; set; }

        public void OnModelCreating(EntityTypeBuilder<UserProfileModel> builder)
        {
            builder.Property(p => p.IsActive).HasDefaultValue(true);
        }
    }
}