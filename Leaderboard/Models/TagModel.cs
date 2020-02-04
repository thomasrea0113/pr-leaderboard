using System.Collections.Generic;
using Leaderboard.Models.Features;
using Leaderboard.Models.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Models
{
    public class TagModel : IDbActive, IDbEntity<TagModel>
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<RelatedTag> RelatedTags { get; set; }

        public void OnModelCreating(EntityTypeBuilder<TagModel> builder)
        {
            builder.HasIndex(e => e.Name).IsUnique();
            builder.Property(e => e.Name).IsRequired();
            builder.Property(e => e.IsActive).HasDefaultValue(true);
        }
    }
}