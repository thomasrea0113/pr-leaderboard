using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Areas.Profiles.Models;
using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Models.Relationships
{
    public class UserLeaderboard : IDbEntity<UserLeaderboard>
    {
        public string UserId { get; set; }
        public virtual UserProfileModel User { get; set; }

        public string LeaderboardId { get; set; }

        public virtual LeaderboardModel Leaderboard { get; set; }

        public static UserLeaderboard Create(UserProfileModel user, LeaderboardModel board)
            => new UserLeaderboard
            {
                // User = user,
                UserId = user.UserId,
                // Leaderboard = board,
                LeaderboardId = board.Id,
            };

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