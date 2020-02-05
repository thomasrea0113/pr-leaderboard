using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Models.Relationships
{
    public class RelatedTag : IDbEntity<RelatedTag>
    {
        public string TagId { get; set; }
        public virtual TagModel Tag { get; set; }

        public string RelatedId { get; set; }
        public virtual TagModel Related { get; set; }

        public static RelatedTag Create(TagModel me, TagModel related)
            => new RelatedTag
            {
                TagId = me.Id,
                RelatedId = related.Id
            };

        public void OnModelCreating(EntityTypeBuilder<RelatedTag> builder)
        {
            builder.HasKey(e => new
            {
                e.TagId,
                e.RelatedId
            });

            builder.HasOne(e => e.Tag)
                .WithMany(e => e.RelatedTags)
                .HasForeignKey(e => e.TagId);

            builder.HasOne(e => e.Related)
                .WithMany(e => e.RelatedToMeTags)
                .HasForeignKey(e => e.RelatedId);
        }
    }
}