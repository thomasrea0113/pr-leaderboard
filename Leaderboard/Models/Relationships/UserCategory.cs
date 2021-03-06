using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Models.Relationships
{
    public class UserCategory : IDbEntity<UserCategory>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string UserId { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }

        public string CategoryId { get; set; }

        [JsonIgnore]
        public virtual Category Category { get; set; }

        public void OnModelCreating(EntityTypeBuilder<UserCategory> builder)
        {
            builder.HasOne(b => b.User)
                .WithMany(b => b.UserCategories)
                .HasForeignKey(b => b.UserId)
                .IsRequired();

            builder.HasOne(b => b.Category)
                .WithMany(b => b.UserCategories)
                .HasForeignKey(b => b.CategoryId)
                .IsRequired();

            builder.Property(b => b.UserId).IsRequired();
            builder.Property(b => b.CategoryId).IsRequired();

            builder.HasIndex(b => new
            {
                b.CategoryId,
                b.UserId
            }).IsUnique();
        }
    }
}