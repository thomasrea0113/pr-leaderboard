using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Areas.Leaderboards.Models
{
    public class WeightClass : IDbEntity<WeightClass>, IOnDbPreSaveAsync
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        /// <summary>
        /// Weight class lower bound in kgs
        /// </summary>
        /// <value></value>
        /// [Range(0, 500)]
        public int? WeightLowerBound { get; set; }

        /// <summary>
        /// weight class upper bound in kgs
        /// </summary>
        /// <value></value>
        /// [Range(0, 500)]
        public int? WeightUpperBound { get; set; }
        public string Range { get; private set; }


        [JsonIgnore]
        public virtual ICollection<DivisionWeightClass> Divisions { get; set; }

        [JsonIgnore]
        public virtual ICollection<LeaderboardModel> Boards { get; set; }

        public void OnModelCreating(EntityTypeBuilder<WeightClass> builder)
        {
            builder.HasIndex(b => new
            {
                b.WeightLowerBound,
                b.WeightUpperBound
            }).IsUnique();
            builder.Property(b => b.Range).IsRequired();
            builder.HasIndex(b => b.Range).IsUnique();
        }

        public Task OnPreSaveAsync(DbContext ctx, PropertyValues values)
        {
            Range = ToString();
            return Task.CompletedTask;
        }

        private string ToBound(int? bound) => bound != null ? Convert.ToString(bound) : "any";

        public override string ToString() => $"{ToBound(WeightLowerBound)}-{ToBound(WeightUpperBound)}";
    }
}