using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Areas.Leaderboards.Models
{
    public class WeightClass : IDbEntity<WeightClass>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        /// <summary>
        /// Weight class lower bound in kgs
        /// </summary>
        /// <value></value>
        /// [Range(0, 500)]
        public int WeightLowerBound { get; set; }

        /// <summary>
        /// weight class upper bound in kgs
        /// </summary>
        /// <value></value>
        /// [Range(0, 500)]
        public int WeightUpperBound { get; set; }

        public virtual ICollection<DivisionWeightClass> Divisions { get; set; }
        public virtual ICollection<LeaderboardModel> Boards { get; set; }

        public void OnModelCreating(EntityTypeBuilder<WeightClass> builder)
        {
            builder.HasIndex(b => new
            {
                b.WeightLowerBound,
                b.WeightUpperBound
            }).IsUnique();
        }

        public override string ToString() => $"{WeightLowerBound}-{WeightUpperBound}";
    }
}