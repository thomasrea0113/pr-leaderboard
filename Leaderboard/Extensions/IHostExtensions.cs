using System;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Data;
using Leaderboard.Data.SeedExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Leaderboard.Extensions
{
    public static class IHostExtensions
    {
        /// <summary>
        /// Migrate the database. It's Important that you don't wrap any services
        /// in a using clause - the scope will likely be reused throughout the
        /// startup process. DI will handle the disposing for us
        /// </summary>
        /// <param name="provider">Service Collection</param>
        /// <param name="env">developement/production/etc.</param>
        /// <param name="seed">whether or not to also run the seed procedure</param>
        /// <returns></returns>
        public static async Task MigrateAsync(this IServiceProvider provider, string env, bool seed = false)
        {
            var ctx = provider.GetRequiredService<ApplicationDbContext>();

            var pending = await ctx.Database.GetPendingMigrationsAsync();
            var current = (await ctx.Database.GetAppliedMigrationsAsync()).LastOrDefault();
            if (pending.Any())
            {
                var migrator = ctx.Database.GetService<IMigrator>();
                await ctx.Database.MigrateAsync();
                if (seed)
                    try
                    {
                        await provider.SeedDataAsync(env);
                    }
                    catch
                    {
                        // if the seed failed, then we want to remove the migration(s) we just added
                        await migrator.MigrateAsync(current ?? "0");
                        throw;
                    }
            }
        }
    }
}
