using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Areas.Leaderboards.Models
{
    public class UnitOfMeasureModel : IDbEntity<UnitOfMeasureModel>, IDbActive
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string Unit { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<LeaderboardModel> Boards { get; set; }

        public void OnModelCreating(EntityTypeBuilder<UnitOfMeasureModel> builder)
        {

            builder.Property(e => e.IsActive).HasDefaultValue(true);

            // each uom must have a unit, and it must be unique
            builder.Property(b => b.Unit).IsRequired();
            builder.HasIndex(b => b.Unit).IsUnique();

            // there are few UOMs, so it makes sense to include them in the migrations
            builder.HasData(new UnitOfMeasureModel
            {
                Id = "e362dd90-d6fe-459b-ba26-09db002bfff6",
                IsActive = true,
                Unit = "Kilograms"
            }, new UnitOfMeasureModel
            {
                Id = "12c7c15a-db13-4912-a7c8-fc86db54849b",
                IsActive = true,
                Unit = "Seconds"
            }, new UnitOfMeasureModel
            {
                Id = "d77c24f6-54f1-448d-9117-ea4e7034904f",
                IsActive = true,
                Unit = "Meters"
            });
        }
    }
}