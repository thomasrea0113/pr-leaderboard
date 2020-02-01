using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Leaderboard.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Leaderboard.Models.Features;
using Leaderboard.Models.Relationships.Extensions;
using Leaderboard.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace Leaderboard.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
    {
        public DbSet<LeaderboardModel> leaderboards { get; set; }
        public DbSet<UserProfileModel> UserProfiles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.AddAllRelationships();
            modelBuilder.AddCompositeKeys();
        }

        private Func<EntityEntry, bool> hasFeature = ee => {
            var e = ee.Entity;
            return e is IOnDbSave || e is IOnDbCreate;
        };

        public override int SaveChanges()
        {
            this.ChangeTracker.DetectChanges();

            var featuredEntries = this.ChangeTracker.Entries().Where(hasFeature);
            
            var added = featuredEntries.Where(t => t.State == EntityState.Added);
            foreach (var entry in added)
            {
                var entity = entry.Entity;
                if (entry.Entity is IOnDbCreate onCreate)
                    onCreate.OnCreate(entry.Context, entry.CurrentValues);
            }

            var modified = featuredEntries.Where(t => t.State == EntityState.Modified);
            foreach (var entry in modified)
            {
                var entity = entry.Entity;

                // additional OnSave functionality
                if (entity is IOnDbSave onSave)
                    onSave.OnSave(entry.Context, entry.CurrentValues);
            }

            var deleted = featuredEntries.Where(t => t.State == EntityState.Deleted);
            foreach (var entry in deleted)
            {
                var entity = entry.Entity;

                // additional actions to take when deleted
                if (entity is IOnDbDelete onDelete)
                    onDelete.OnDelete(entry.Context);

                // if this model has an active feature, then we prevent the delete
                // and set active to false
                if (entity is IDbActive active)
                {
                    active.IsActive = false;
                    entry.State = EntityState.Modified;
                }

                // prevents the model from being deleted
                // TODO notify user somewhere of prevents deletion?
                if (entity is IModelFeatures featured)
                {
                    if (featured.Features.HasFlag(ModelFeatures.PreventDelete))
                        entry.State = EntityState.Unchanged;
                }
            }

            return base.SaveChanges();
        }
    }
}
