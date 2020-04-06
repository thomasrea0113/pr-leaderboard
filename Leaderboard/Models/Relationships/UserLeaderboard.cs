using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Models.Relationships
{
    public class UserLeaderboard : IDbEntity<UserLeaderboard>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string UserId { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }

        public string LeaderboardId { get; set; }

        [JsonIgnore]
        public virtual LeaderboardModel Leaderboard { get; set; }

        public static UserLeaderboard Create(ApplicationUser user, LeaderboardModel board)
            => new UserLeaderboard
            {
                Id = System.Guid.NewGuid().ToString(),
                // User = user,
                UserId = user.Id,
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
            builder.HasIndex(e => new
            {
                e.UserId,
                e.LeaderboardId
            }).IsUnique();
        }
    }
}