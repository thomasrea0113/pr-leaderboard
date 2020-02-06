using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Models.Features;
using Leaderboard.Models.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Areas.Profiles.Models
{
    public class UserProfileModel : IDbActive, IDbEntity<UserProfileModel>
    {
        [Key]
        public string UserId { get; set; }

        // This can't be lazy loaded, because the IdentityUser model doesn't have a Profile property
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<UserLeaderboard> UserLeaderboards { get; set; } = new List<UserLeaderboard>();

        public bool? IsActive { get; set; }

        public void OnModelCreating(EntityTypeBuilder<UserProfileModel> builder)
        {
            builder.Property(p => p.IsActive).HasDefaultValue(true);
        }
    }
}