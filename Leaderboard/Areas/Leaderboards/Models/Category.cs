using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Leaderboard.Models.Features;
using Leaderboard.Models.Relationships;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Areas.Leaderboards.Models
{
    public class Category : IDbEntity<Category>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<DivisionCategory> DivisionCategories { get; set; } = new List<DivisionCategory>();
        public virtual ICollection<UserCategory> UserCategories { get; set; } = new List<UserCategory>();

        public void OnModelCreating(EntityTypeBuilder<Category> builder)
        {
            builder.Property(b => b.Name).IsRequired();
            builder.HasIndex(b => b.Name).IsUnique();

            builder.HasData(new Category {
                Id = "642313a2-1f0c-4329-a676-7a9cdac045bd",
                Name = "Powerlifting"
            },new Category {
                Id = "9edc53a6-34ec-4cde-8eb0-cac009579b72",
                Name = "Weightlifting"
            },new Category {
                Id = "6772a358-e5b7-49dd-a49b-9d855ed46c5e",
                Name = "Running"
            });
        }
    }
}