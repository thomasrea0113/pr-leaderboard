using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Areas.Leaderboards.Models
{
    public class UnitOfMeasureModel : IDbEntity<UnitOfMeasureModel>, IDbActive
    {
        public string Unit { get; set; }

        public bool? IsActive { get; set; }

        public void OnModelCreating(EntityTypeBuilder<UnitOfMeasureModel> builder)
        {
            builder.HasKey(e => e.Unit);
            builder.Property(e => e.IsActive).HasDefaultValue(true);
        }
    }
}