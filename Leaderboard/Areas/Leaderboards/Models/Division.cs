using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Collections.Generic;
using Leaderboard.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Areas.Leaderboards.Models
{
    public class Division : IDbEntity<Division>
    {
        public enum GenderValues { Male, Female, Other, All }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public GenderValues Gender { get; set; }
        public string Name { get; set; }

        [Range(0, 500)]
        public int AgeLowerBound { get; set; }
        [Range(0, 500)]
        public int AgeUpperBound { get; set; }

        public virtual ICollection<DivisionWeightClass> WeightClasses { get; set; }

        public virtual ICollection<LeaderboardModel> Boards { get; set; }

        public virtual List<RelatedDivision> RelatedDivisions { get; set; } = new List<RelatedDivision>();
        public virtual List<RelatedDivision> DivisionsRelatedToMe { get; set; } = new List<RelatedDivision>();

        /// <summary>
        /// All related divisions, including what has added the current tag as a related tag
        /// </summary>
        /// <param name="t.Related"></param>
        /// <returns></returns>
        [NotMapped]
        public List<Division> AllRelatedDivisions => RelatedDivisions.Select(t => t.Related)
            .Union(DivisionsRelatedToMe.Select(t => t.Division))
            .OrderBy(t => t.Name)
            .ToList();

        public void OnModelCreating(EntityTypeBuilder<Division> builder)
        {
            // stores the enum value as a string in the database
            builder.Property(b => b.Gender).HasDefaultValue(GenderValues.All).HasConversion<string>();
            builder.Property(b => b.Name).IsRequired();

            // can't have multiple divisions with the same name for a given gender
            builder.HasIndex(b => new
            {
                b.Gender,
                b.Name
            }).IsUnique();
        }

        public override string ToString()
        {
            return $"{Name} ({Gender})";
        }
    }
}