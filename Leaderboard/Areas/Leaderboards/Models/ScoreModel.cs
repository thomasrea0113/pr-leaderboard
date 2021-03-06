using Newtonsoft.Json;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using Leaderboard.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Leaderboard.Areas.Leaderboards.Models
{
    public class ScoreModel : IDbEntity<ScoreModel>, IOnDbPreCreateAsync
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public DateTime? ApprovedDate { get; set; }
        public bool IsApproved => ApprovedDate != null;

        public string BoardId { get; set; }

        [JsonIgnore]
        public virtual LeaderboardModel Board { get; set; }

        public string UserId { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }

        public decimal Value { get; set; }

        public DateTime CreatedDate { get; set; }

        public void OnModelCreating(EntityTypeBuilder<ScoreModel> builder)
        {
            builder.HasOne(e => e.Board)
                .WithMany(e => e.Scores)
                .HasForeignKey(e => e.BoardId);

            builder.HasOne(e => e.User)
                .WithMany(e => e.Scores)
                .HasForeignKey(e => e.UserId);

            builder.Property(b => b.CreatedDate)
                .HasConversion(Conversions.LocalToUtcDateTime)
                .IsRequired();

            builder.Property(b => b.ApprovedDate)
                .HasConversion(Conversions.LocalToUtcDateTime);

            // ensuring the value is always accurate to 4 decimal places
            builder.Property(e => e.Value).HasColumnType("decimal(12,4)");

            builder.Property(e => e.BoardId).ValueGeneratedNever().IsRequired();
            builder.Property(e => e.UserId).ValueGeneratedNever().IsRequired();

            builder.HasKey(e => e.Id);
        }

        public Task OnPreCreateAsync(DbContext ctx, PropertyValues values)
        {
            if (CreatedDate > DateTime.UtcNow)
                throw new InvalidOperationException($"{nameof(CreatedDate)} must be earlier than the current time");
            if (CreatedDate == default)
                CreatedDate = DateTime.UtcNow;

            if (ApprovedDate <= CreatedDate)
                throw new InvalidOperationException($"{nameof(CreatedDate)} must be earlier than {nameof(ApprovedDate)}");

            return Task.CompletedTask;
        }
    }
}