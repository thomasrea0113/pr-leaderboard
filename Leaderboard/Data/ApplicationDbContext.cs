﻿using System;
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

namespace Leaderboard.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<LeaderboardModel> leaderboards { get; set; }
        public DbSet<UserProfileModel> UserProfiles { get; set; }

        #region relationship tables

        public DbSet<UserLeaderboard> UserLeaderboards { get; set; }

        #endregion

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            this.ConfigureEntities(modelBuilder);
            modelBuilder.EnableAutoHistory(null);
        }

        private Func<EntityEntry, bool> hasFeature = ee => {
            var e = ee.Entity;
            return e is IOnDbSave || e is IOnDbPreCreateAsync;
        };

        public override int SaveChanges()
            => throw new NotImplementedException("Please use the SaveChangesAsync method.");

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            this.ChangeTracker.DetectChanges();

            var allEntries = this.ChangeTracker.Entries();

            // All added users. need to evaluate the enumerable immediately
            var users = allEntries.Where(ee => ee.State == EntityState.Added)
                .Select(ee => ee.Entity)
                .Where(e => e is IdentityUser)
                .Select(e => (IdentityUser)e)
                .ToArray();

            // TODO getting all the users prevents multiple DB calls, but could
            // be problematic with a large number of users
            var userProfiles = await UserProfiles.ToListAsync();

            // any time a user is created, make sure a profile for them also exists
            foreach (var user in users)
                if (!userProfiles.Any(p => p.UserId == user.Id))
                    await UserProfiles.AddAsync(new UserProfileModel { UserId = user.Id });
            
            var added = allEntries.Where(t => t.State == EntityState.Added);
            foreach (var entry in added)
            {
                var entity = entry.Entity;
                if (entry.Entity is IOnDbPreCreateAsync onCreate)
                    await onCreate.OnPreCreateAsync(entry.Context, entry.CurrentValues);
            }

            var modified = allEntries.Where(t => t.State == EntityState.Modified);
            foreach (var entry in modified)
            {
                var entity = entry.Entity;

                // additional OnSave functionality
                if (entity is IOnDbSave onSave)
                    onSave.OnSave(entry.Context, entry.CurrentValues);
            }

            var deleted = allEntries.Where(t => t.State == EntityState.Deleted);
            foreach (var entry in deleted)
            {
                var entity = entry.Entity;

                // additional actions to take when deleted
                if (entity is IOnDbDelete onDelete)
                    onDelete.OnDelete(entry.Context);

                // if this model has an active feature, then we prevent the delete
                // and set active to false
                // TODO notify user somewhere of prevents deletion?
                if (entity is IDbActive active)
                {
                    active.IsActive = false;
                    entry.State = EntityState.Modified;
                }

                if (entity is IModelFeatures featured)
                {
                    
                }
            }

            this.EnsureAutoHistory();
            
            return await base.SaveChangesAsync();
        }
    }
}
