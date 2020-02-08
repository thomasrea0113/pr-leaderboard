using System;
using System.Linq;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Data.SeedExtensions;
using Leaderboard.Models.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Areas.Leaderboards.Models
{
    public class ScoreModel : IDbEntity<ScoreModel>
    {
        public string Id { get; set; }

        public string BoardId { get; set; }
        public virtual LeaderboardModel Board { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public decimal Value { get; set; }

        public void OnModelCreating(EntityTypeBuilder<ScoreModel> builder)
        {
            builder.HasOne(e => e.Board)
                .WithMany(e => e.Scores)
                .HasForeignKey(e => e.BoardId);

            builder.HasOne(e => e.User)
                .WithMany(e => e.Scores)
                .HasForeignKey(e => e.UserId);

            // ensuring the value is always accurate to 4 decimal places
            builder.Property(e => e.Value).HasColumnType("decimal(12,4)");

            builder.Property(e => e.BoardId).ValueGeneratedNever().IsRequired();
            builder.Property(e => e.UserId).ValueGeneratedNever().IsRequired();

            builder.HasKey(e => e.Id);
        }
    }
}