using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Leaderboard.Models;
using Leaderboard.Models.Identity;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Leaderboard.Models.Features;
using Leaderboard.Models.Relationships.Extensions;

namespace Leaderboard.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserModel, RoleModel, Guid>
    {
        public DbSet<LeaderboardModel> leaderboards { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddAllRelationships();
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
                if (entity is IOnDbSave onSave)
                    onSave.OnSave(entry.Context, entry.CurrentValues);
            }

            var deleted = featuredEntries.Where(t => t.State == EntityState.Deleted);
            foreach (var entry in deleted)
            {
                var entity = entry.Entity;
                if (entity is IOnDbDelete onDelete)
                    onDelete.OnDelete(entry.Context);
            }

            return base.SaveChanges();
        }
    }
}
