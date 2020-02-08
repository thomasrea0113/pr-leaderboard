using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Leaderboard.Models.Features;
using Leaderboard.Models.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Areas.Leaderboards.Models
{
    // TODO implement sluggy on save
    public class LeaderboardModel : IDbEntity<LeaderboardModel>, IDbActive
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public string DivisionId { get; set; }
        public virtual Division Division { get; set; }


        public virtual ICollection<UserLeaderboard> UserLeaderboards { get; set; } = new List<UserLeaderboard>();

        public bool? IsActive { get; set; }

        public virtual ICollection<ScoreModel> Scores { get; set; }

        public static string[] SeedIds { get; } = new string[]
        {
            "61c6fe69-0be4-4d4e-bdca-3bc641b4402a",
            "95ffb9c3-2122-410a-ba44-272f2188ed56",
            "1c161800-801e-492b-9053-e01203d63490"
        };

        public void OnModelCreating(EntityTypeBuilder<LeaderboardModel> builder)
        {
            // ensuring Name is unique
            builder.HasIndex(p => p.Name).IsUnique();
            builder.Property(p => p.IsActive).HasDefaultValue(true);

            // a board has one division. When we delete the board, we don't
            // want to automatically delete the division as well.
            builder.HasOne(b => b.Division)
                .WithOne(b => b.Board)
                .HasForeignKey<Division>(b => b.BoardId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasData(new LeaderboardModel
            {
                Id = SeedIds[0].ToString(),
                Name = "Deadlift 1 Rep Max",
                IsActive = true
            }, new LeaderboardModel
            {
                Id = SeedIds[1].ToString(),
                Name = "Bench 1 Rep Max",
                IsActive = true
            }, new LeaderboardModel
            {
                Id = SeedIds[2].ToString(),
                Name = "Squat 1 Rep Max",
                IsActive = true
            });
        }
    }
}