using Leaderboard.Models.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Areas.Leaderboards.Models
{
    public class ScoreModel : IDbEntity<ScoreModel>
    {
        public string BoardId { get; set; }
        public virtual LeaderboardModel Board { get; set; }

        public void OnModelCreating(EntityTypeBuilder<ScoreModel> builder)
        {
            builder.HasOne(e => e.Board)
                .WithMany(e => e.Scores)
                .HasForeignKey(e => e.BoardId)
                .IsRequired();

            builder.Property(e => e.BoardId).ValueGeneratedNever();
            builder.HasKey(e => e.BoardId);
        }
    }
}