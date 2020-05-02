using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Leaderboard.Models.Features;
using Leaderboard.Models.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Areas.Leaderboards.Models
{
    // TODO implement sluggy on save
    public class LeaderboardModel : IDbEntity<LeaderboardModel>, IDbActive, ISlugged
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public string DivisionId { get; set; }

        [JsonIgnore]
        public virtual Division Division { get; set; }

        public string WeightClassId { get; set; }

        [JsonIgnore]
        public virtual WeightClass WeightClass { get; set; }


        [JsonIgnore]
        public virtual ICollection<UserLeaderboard> UserLeaderboards { get; set; } = new List<UserLeaderboard>();

        public bool IsActive { get; set; } = true;

        [JsonIgnore]
        public virtual ICollection<ScoreModel> Scores { get; set; }

        public virtual string UOMId { get; set; }

        [JsonIgnore]
        public virtual UnitOfMeasureModel UOM { get; set; }

        public string Slug { get; set; }

        public void OnModelCreating(EntityTypeBuilder<LeaderboardModel> builder)
        {
            // ensuring Name is unique
            builder.Property(p => p.Name).IsRequired();
            builder.Property(b => b.Slug).IsRequired();

            // A division can only contain one board with a given name
            builder.HasIndex(p => new
            {
                p.DivisionId,
                p.WeightClassId,
                p.Slug
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

        #region Helper Methods

        /// <summary>
        /// The 'View Board' page uses a friendly url, which means this information is needed to route currectly with Url.Page()
        /// </summary>
        /// <returns></returns>
        public object GetViewArgs() => new
        {
            area = "Leaderboards",
            division = Division.Slug,
            gender = Division.Gender?.ToString().ToLower() ?? "any",
            weightClass = WeightClass?.Range ?? "any",
            slug = Slug
        };

        #endregion

        #region equality

        // 2 instances are equal if they have the same Id
#nullable enable
        public override int GetHashCode() => Id.GetHashCode();
        public override bool Equals(object? obj)
        {
            if (obj is LeaderboardModel m)
                return Id.Equals(m.Id);
            return Id.Equals(obj);
        }
#nullable disable

        #endregion
    }
}