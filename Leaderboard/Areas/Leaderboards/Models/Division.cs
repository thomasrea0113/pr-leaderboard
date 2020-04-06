using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Collections.Generic;
using Leaderboard.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Leaderboard.Areas.Identity.Models;
using Newtonsoft.Json;
using static Leaderboard.Utilities.SlugUtilities;

namespace Leaderboard.Areas.Leaderboards.Models
{
    public class Division : IDbEntity<Division>, ISlugged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public GenderValues? Gender { get; set; }
        public string Name { get; set; }

        // public string IconId { get; set; }

        // [JsonIgnore]
        // public FileModel Icon { get; set; }

        [Range(0, 500)]
        public int? AgeLowerBound { get; set; }
        [Range(0, 500)]
        public int? AgeUpperBound { get; set; }

        [JsonIgnore]
        public virtual ICollection<DivisionWeightClass> WeightClasses { get; set; }

        [JsonIgnore]
        public virtual ICollection<LeaderboardModel> Boards { get; set; }

        [JsonIgnore]
        public virtual ICollection<DivisionCategory> DivisionCategories { get; set; }

        public string Slug { get; set; }

        public void OnModelCreating(EntityTypeBuilder<Division> builder)
        {
            // stores the enum value as a string in the database
            builder.Property(b => b.Gender).HasConversion<string>();

            builder.Property(b => b.Name).IsRequired();
            builder.Property(b => b.Slug).IsRequired();

            // can't have multiple divisions with the same name for a given gender
            builder.HasIndex(b => new
            {
                b.Gender,
                b.Slug
            }).IsUnique();
        }

        public override string ToString()
        {
            return $"{Name} ({Gender})";
        }
    }
}