using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Data;
using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Models
{
    public class FileModel : IDbEntity<FileModel>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        /// <summary>
        /// The relative path to the file. Should be file provider
        /// and OS agnostic
        /// </summary>
        /// <value></value>
        public string Path { get; set; }
        public DateTime UploadDate { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        public void OnModelCreating(EntityTypeBuilder<FileModel> builder)
        {
            // the path should be unique across all files on the server
            builder.Property(b => b.Path).IsRequired();
            builder.HasIndex(b => b.Path).IsUnique();

            // all dates will be stored as UTC
            builder.Property(b => b.UploadDate)
                .HasConversion(Conversions.LocalToUtcDateTime)
                .IsRequired();

            builder.HasOne(b => b.CreatedBy)
                .WithMany(b => b.UploadedFiles)
                .HasForeignKey(b => b.UserId)
                .IsRequired();
        }
    }
}