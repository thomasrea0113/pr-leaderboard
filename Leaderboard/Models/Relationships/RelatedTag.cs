using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Models.Relationships
{
    public class RelatedTag : IDbEntity<RelatedTag>
    {
        public string Id { get; set; }

        public string TagId { get; set; }
        public virtual TagModel Tag { get; set; }

        public string RelatedId { get; set; }
        public virtual TagModel Related { get; set; }

        public void OnModelCreating(EntityTypeBuilder<RelatedTag> builder)
        {
            builder.Property(u => u.RelatedId)
                .ValueGeneratedNever()
                .IsRequired();
                
            builder.Property(u => u.TagId)
                .ValueGeneratedNever()
                .IsRequired();

            builder.HasKey(e => new {
                e.TagId,
                e.RelatedId
            });

            builder.HasOne(e => e.Related)
                .WithMany(e => e.RelatedTags)
                .HasForeignKey(e => e.RelatedId);
        }
    }
}