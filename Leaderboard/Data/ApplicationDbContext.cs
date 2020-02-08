using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Leaderboard.Models.Features;
using Leaderboard.Models.Relationships;
using System.Threading.Tasks;
using System.Threading;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Models;
using Leaderboard.Areas.Identity.Models;
using Microsoft.Extensions.Configuration;
using Leaderboard.Data.SeedExtensions;
using Leaderboard.Areas.Identity.Managers;

namespace Leaderboard.Data
{
    public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser, ApplicationRole, string,
        ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
        ApplicationRoleClaim, ApplicationUserToken>

    {
        public DbSet<LeaderboardModel> leaderboards { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<ScoreModel> Scores { get; set; }
        public DbSet<UnitOfMeasureModel> UnitsOfMeasure { get; set; }

        #region relationship tables

        public DbSet<UserLeaderboard> UserLeaderboards { get; set; }
        public DbSet<RelatedDivision> RelatedDivisions { get; set; }

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

        public override int SaveChanges()
            => throw new NotImplementedException("Please use the SaveChangesAsync method.");

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            this.ChangeTracker.DetectChanges();

            var allEntries = this.ChangeTracker.Entries();

            // All added users. need to evaluate the enumerable immediately
            var users = allEntries.Select(ee => ee.Entity)
                .Where(e => e is ApplicationUser)
                .Cast<ApplicationUser>()
                .ToArray();

            await this.ProcessPreSaveFeaturesAsync(allEntries);

            var added = allEntries.Where(s => s.State == EntityState.Added);
            
            var count = await base.SaveChangesAsync();

            await this.ProcessPostSaveFeaturesAsync(allEntries);

            return count;
        }
    }
}
