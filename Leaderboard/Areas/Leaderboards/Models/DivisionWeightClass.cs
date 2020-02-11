using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Areas.Leaderboards.Models
{
    public class DivisionWeightClass : IDbEntity<DivisionWeightClass>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string DivisionId { get; set; }
        public virtual Division Division { get; set; }

        public string WeightClassId { get; set; }
        public virtual WeightClass WeightClass { get; set; }

        public void OnModelCreating(EntityTypeBuilder<DivisionWeightClass> builder)
        {
            builder.HasOne(b => b.Division)
                .WithMany(b => b.WeightClasses)
                .HasForeignKey(b => b.DivisionId)
                .IsRequired();

            builder.HasOne(b => b.WeightClass)
                .WithMany(b => b.Divisions)
                .HasForeignKey(b => b.WeightClassId)
                .IsRequired();

            builder.HasIndex(b => new
            {
                b.DivisionId,
                b.WeightClassId
            }).IsUnique();
        }
    }
}