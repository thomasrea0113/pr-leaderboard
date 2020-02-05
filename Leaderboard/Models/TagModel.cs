using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Leaderboard.Models.Features;
using Leaderboard.Models.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Models
{
    public class TagModel : IDbActive, IDbEntity<TagModel>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string Name { get; set; }
        public bool? IsActive { get; set; }

        public virtual List<RelatedTag> RelatedTags { get; set; } = new List<RelatedTag>();
        public virtual List<RelatedTag> RelatedToMeTags { get; set; } = new List<RelatedTag>();

        public void OnModelCreating(EntityTypeBuilder<TagModel> builder)
        {
            builder.HasIndex(e => e.Name).IsUnique();
            builder.Property(e => e.Name).IsRequired();
            builder.Property(e => e.IsActive).HasDefaultValue(true);
        }
    }
}