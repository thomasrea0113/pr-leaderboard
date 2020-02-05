using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Leaderboard.Models.Features;
using Microsoft.AspNetCore.Identity;
using Leaderboard.Models.Relationships;
using System.Threading.Tasks;
using System.Threading;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Areas.Profiles.Models;
using Leaderboard.Areas.Profiles.DbContextExtensions;
using Leaderboard.Models;
using System.Collections.Generic;

namespace Leaderboard.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<LeaderboardModel> leaderboards { get; set; }
        public DbSet<UserProfileModel> UserProfiles { get; set; }

        public DbSet<TagModel> Tags { get; set; }
        public DbSet<ScoreModel> Scores { get; set; }
        public DbSet<UnitOfMeasureModel> UnitsOfMeasure { get; set; }

        #region relationship tables

        public DbSet<UserLeaderboard> UserLeaderboards { get; set; }
        public DbSet<RelatedTag> RelatedTags { get; set; }

        #endregion

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            this.ConfigureEntities(modelBuilder);
        }

        private Func<EntityEntry, bool> hasFeature = ee => {
            var e = ee.Entity;
            return e is IOnDbPreSaveAsync || e is IOnDbPreCreateAsync;
        };

        public override int SaveChanges()
            => throw new NotImplementedException("Please use the SaveChangesAsync method.");

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            this.ChangeTracker.DetectChanges();

            var allEntries = this.ChangeTracker.Entries();

            // All added users. need to evaluate the enumerable immediately
            var users = allEntries.Select(ee => ee.Entity)
                .Where(e => e is IdentityUser)
                .Cast<IdentityUser>()
                .ToArray();

            await this.EnsureProfilesAsync(users);
            await this.ProcessPreSaveFeaturesAsync(allEntries);

            var added = allEntries.Where(s => s.State == EntityState.Added);
            
            var count = await base.SaveChangesAsync();

            await this.ProcessPostSaveFeaturesAsync(allEntries);

            return count;
        }
    }
}
