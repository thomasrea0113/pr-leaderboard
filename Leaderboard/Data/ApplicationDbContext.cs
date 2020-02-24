using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Leaderboard.Models.Features;
using Leaderboard.Models.Relationships;
using System.Threading.Tasks;
using System.Threading;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Models;
using Leaderboard.Areas.Identity.Models;

namespace Leaderboard.Data
{
    public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser, ApplicationRole, string,
        ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
        ApplicationRoleClaim, ApplicationUserToken>

    {
        public DbSet<LeaderboardModel> Leaderboards { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<WeightClass> WeightClasses { get; set; }
        public DbSet<ScoreModel> Scores { get; set; }
        public DbSet<UnitOfMeasureModel> UnitsOfMeasure { get; set; }
        public DbSet<FileModel> UploadedFiles { get; set; }
        public DbSet<Category> Categories { get; set; }

        #region relationship tables

        public DbSet<UserLeaderboard> UserLeaderboards { get; set; }
        public DbSet<DivisionCategory> DivisionCategories { get; set; }
        public DbSet<UserCategory> UserCategories { get; set; }
        public DbSet<DivisionWeightClass> DivisionWeightClasses { get; set; }

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

            await allEntries.ProcessPreSaveFeaturesAsync();

            var added = allEntries.Where(s => s.State == EntityState.Added);
            
            var count = await base.SaveChangesAsync();

            await allEntries.ProcessPostSaveFeaturesAsync();

            return count;
        }
    }
}
