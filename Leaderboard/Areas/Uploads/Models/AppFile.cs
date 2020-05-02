using System;
using System.ComponentModel.DataAnnotations;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Data;
using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Areas.Uploads.Models
{
    public class AppFile : IDbEntity<AppFile>
    {
        public int Id { get; set; }

        /// <summary>
        /// size in bytes
        /// </summary>
        /// <value></value>
        [Display(Name = "Size (bytes)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long Size { get; set; }

        /// <summary>
        /// The relative path to the file. Should be file provider
        /// and OS agnostic
        /// </summary>
        /// <value></value>
        public string Path { get; set; }
        public DateTime UploadDate { get; set; }

        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        public void OnModelCreating(EntityTypeBuilder<AppFile> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).ValueGeneratedOnAdd();

            builder.Property(b => b.Size).IsRequired();

            // the path should be unique across all files on the server
            builder.Property(b => b.Path).IsRequired();
            builder.HasIndex(b => b.Path).IsUnique();

            // all dates will be stored as UTC
            builder.Property(b => b.UploadDate)
                .HasConversion(Conversions.LocalToUtcDateTime)
                .IsRequired();

            builder.HasOne(b => b.CreatedBy)
                .WithMany(b => b.UploadedFiles)
                .HasForeignKey(b => b.CreatedById)
                .IsRequired();
        }
    }
}
