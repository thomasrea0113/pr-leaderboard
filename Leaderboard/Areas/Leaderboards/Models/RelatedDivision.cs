using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Leaderboard.Models;
using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Areas.Leaderboards.Models
{
    public class RelatedDivision : IDbEntity<RelatedDivision>
    {
        public string DivisionId { get; set; }
        public virtual Division Division { get; set; }

        public string RelatedId { get; set; }
        public virtual Division Related { get; set; }

        public static RelatedDivision Create(Division me, Division related)
            => new RelatedDivision
            {
                DivisionId = me.Id,
                RelatedId = related.Id
            };

        public void OnModelCreating(EntityTypeBuilder<RelatedDivision> builder)
        {
            builder.HasKey(e => new
            {
                e.DivisionId,
                e.RelatedId
            });

            builder.HasOne(e => e.Division)
                .WithMany(e => e.RelatedDivisions)
                .HasForeignKey(e => e.DivisionId);

            builder.HasOne(e => e.Related)
                .WithMany(e => e.DivisionsRelatedToMe)
                .HasForeignKey(e => e.RelatedId);
        }
    }
}