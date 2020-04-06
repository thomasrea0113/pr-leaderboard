using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Areas.Leaderboards.Models
{
    public class DivisionCategory : IDbEntity<DivisionCategory>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string DivisionId { get; set; }

        [JsonIgnore]
        public virtual Division Division { get; set; }

        public string CategoryId { get; set; }

        [JsonIgnore]
        public virtual Category Category { get; set; }

        public void OnModelCreating(EntityTypeBuilder<DivisionCategory> builder)
        {
            builder.HasOne(b => b.Division)
                .WithMany(b => b.DivisionCategories)
                .HasForeignKey(b => b.DivisionId);

            builder.HasOne(b => b.Category)
                .WithMany(b => b.DivisionCategories)
                .HasForeignKey(b => b.CategoryId);

            builder.Property(b => b.DivisionId).IsRequired();
            builder.Property(b => b.CategoryId).IsRequired();

            builder.HasIndex(b => new
            {
                b.CategoryId,
                b.DivisionId
            }).IsUnique();
        }
    }
}