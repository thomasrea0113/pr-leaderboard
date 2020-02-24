using System;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Data;
using Leaderboard.Data.SeedExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Leaderboard.Extensions
{
    public static class IHostExtensions
    {
        public static async Task<IHost> MigrateAsync(this IHost host, string env, bool seed = false)
        {
            var provider = host.Services.CreateScope().ServiceProvider;
            var ctx = provider.GetRequiredService<ApplicationDbContext>();
            
            if ((await ctx.Database.GetPendingMigrationsAsync()).Any())
            {
                await ctx.Database.MigrateAsync();
                if (seed)
                    await provider.SeedDataAsync(env);
            }

            return host;
        }
    }
}
