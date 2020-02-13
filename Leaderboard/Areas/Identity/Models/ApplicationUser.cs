using System;
using System.Collections.Generic;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Data;
using Leaderboard.Models;
using Leaderboard.Models.Features;
using Leaderboard.Models.Relationships;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Leaderboard.Utilities.Extensions;

namespace Leaderboard.Areas.Identity.Models
{
    public enum GenderValues
    {
        Male,
        Female,
        Other
    }

    public class ApplicationUser : IdentityUser, IDbEntity<ApplicationUser>, IDbActive
    {
        public GenderValues? Gender { get; set; }
        public decimal? Weight { get; set; }

        public DateTime? BirthDate { get; set; }

        public int? Age => BirthDate?.GetAge();


        public virtual ICollection<UserLeaderboard> UserLeaderboards { get; set; } = new List<UserLeaderboard>();
        public virtual ICollection<ScoreModel> Scores { get; set; } = new List<ScoreModel>();
        public virtual ICollection<FileModel> UploadedFiles { get; set; } = new List<FileModel>();
        
        public virtual ICollection<DivisionCategory> Interests { get; set; } = new List<DivisionCategory>();

        public bool? IsActive { get; set; }

        #region identity relationships

        public virtual ICollection<ApplicationUserClaim> Claims { get; set; }
        public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
        public virtual ICollection<ApplicationUserToken> Tokens { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

        #endregion

        public ApplicationUser()
        {
        }

        public ApplicationUser(string userName) : base(userName)
        {
        }

        public void OnModelCreating(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(b => b.Gender)
                .HasDefaultValue(GenderValues.Other)
                .IsRequired();

            builder.Property(b => b.Weight).HasColumnType("decimal(13,3)");

            builder.Property(b => b.BirthDate)
                .HasConversion(Conversions.LocalToUtcDateTime);

            builder.HasMany(b => b.Interests)
                .WithOne();

            // Each User can have many UserClaims
            builder.HasMany(e => e.Claims)
                .WithOne(e => e.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

            // Each User can have many UserLogins
            builder.HasMany(e => e.Logins)
                .WithOne(e => e.User)
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            // Each User can have many UserTokens
            builder.HasMany(e => e.Tokens)
                .WithOne(e => e.User)
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            // Each User can have many entries in the UserRole join table
            builder.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.Property(p => p.IsActive).HasDefaultValue(true);
        }
    }
}