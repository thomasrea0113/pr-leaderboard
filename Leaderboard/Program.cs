using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Leaderboard
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var config = services.GetRequiredService<IOptions<AppConfiguration>>().Value;
                var env = services.GetRequiredService<IWebHostEnvironment>().EnvironmentName;

                // TODO log seed, catch Migrate exception and notify user that no changes to the database were applied
                if (config.AutoMigrate.Enabled)
                    await services.MigrateAsync(env, config.AutoMigrate.AutoSeed).ConfigureAwait(false);

                // make sure the admin users in the appSettings are in the admin role
                await services.GetRequiredService<AppUserManager>()
                    .EnsureAdminUsersAsync(config.AdminUsers.ToArray()).ConfigureAwait(false);
            }

            await host.RunAsync().ConfigureAwait(false);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging(o => o.AddDebug())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
