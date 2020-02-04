using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Models.Relationships
{
    public class RelatedTag : IDbEntity<RelatedTag>
    {
        public string TagId { get; set; }
        public virtual TagModel Tag { get; set; }

        public void OnModelCreating(EntityTypeBuilder<RelatedTag> builder)
        {
            builder.Property(u => u.TagId).ValueGeneratedNever();
            builder.HasKey(e => e.TagId);

            builder.HasOne(e => e.Tag)
                .WithMany(e => e.RelatedTags)
                .HasForeignKey(e => e.TagId);
        }
    }
}