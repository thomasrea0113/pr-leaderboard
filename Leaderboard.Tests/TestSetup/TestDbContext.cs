using Leaderboard.Data;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Tests.TestSetup
{
    public class TestDbContext : ApplicationDbContext
    {
        public TestDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableDetailedErrors().EnableSensitiveDataLogging();
        }
    }
}
