using System;
using System.ComponentModel.DataAnnotations;
using Leaderboard.Models.Features;
using Leaderboard.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Models.Relationships
{
    public class UserLeaderboard : IDbEntity<UserLeaderboard>
    {
        public Guid UserId { get; set; }
        public virtual UserProfileModel User { get; set; }


        public Guid LeaderboardId { get; set; }
        public virtual LeaderboardModel Leaderboard { get; set; }

        public void OnModelCreating(EntityTypeBuilder<UserLeaderboard> builder)
        {
            // User -> Leaderboard many-to-many
            builder.HasOne(pt => pt.User)
                .WithMany(p => p.UserLeaderboards)
                .HasForeignKey(pt => pt.UserId);

            // Leaderboard -> User many-to-many
            builder.HasOne(pt => pt.Leaderboard)
                .WithMany(t => t.UserLeaderboards)
                .HasForeignKey(pt => pt.LeaderboardId);

            // composite key
            builder.HasKey(e => new
            {
                e.UserId,
                e.LeaderboardId
            });
        }
    }
}