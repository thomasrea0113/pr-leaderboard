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

        public string WeightClassId { get; set; }
        public virtual WeightClass WeightClass { get; set; }


        public virtual ICollection<UserLeaderboard> UserLeaderboards { get; set; } = new List<UserLeaderboard>();

        public bool? IsActive { get; set; }

        public virtual ICollection<ScoreModel> Scores { get; set; }

        public virtual string UOMId { get; set; }
        public virtual UnitOfMeasureModel UOM { get; set; }

        public void OnModelCreating(EntityTypeBuilder<LeaderboardModel> builder)
        {
            // ensuring Name is unique
            builder.Property(p => p.Name).IsRequired();

            // A division can only contain one board with a given name
            builder.HasIndex(p => new 
            {
                p.DivisionId,
                p.WeightClassId,
                p.Name
            }).IsUnique();

            builder.Property(p => p.IsActive).HasDefaultValue(true);

            // all boards must have a unit of measure. If you try and delete a
            // unit of measure while it is in use by a board, we will prevent it.
            builder.HasOne(b => b.UOM)
                .WithMany(b => b.Boards)
                .HasForeignKey(b => b.UOMId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // a board has one division, but a division has many boards
            builder.HasOne(b => b.Division)
                .WithMany(b => b.Boards)
                .HasForeignKey(b => b.DivisionId)
                .IsRequired();

            // a board has one weight class, but a weight class has many boards
            builder.HasOne(b => b.WeightClass)
                .WithMany(b => b.Boards)
                .HasForeignKey(b => b.WeightClassId);
        }
    }
}