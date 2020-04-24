using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Leaderboard
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var config = services.GetRequiredService<IConfiguration>();
            var env = services.GetRequiredService<IWebHostEnvironment>().EnvironmentName;

            // TODO log seed, catch Migrate exception and notify user that no changes to the database were applied
            if (config.GetValue("AutoMigrate:Enabled", false))
            {
                await services.MigrateAsync(env, config.GetValue("AutoMigrate:AutoSeed", false));
            }

            scope.Dispose();

            // migrate may dispose the previous scope, so we create another
            scope = host.Services.CreateScope();
            services = scope.ServiceProvider;

            await EnsureAdminUsersAsync(services.GetRequiredService<AppUserManager>(), config.GetSection("AdminUsers").Get<string[]>());

            scope.Dispose();

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging(o => o.AddDebug())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static async Task EnsureAdminUsersAsync(AppUserManager userManager, params string[] userNames)
        {
            foreach (var username in userNames)
            {
                var user = await userManager.FindByNameAsync(username);
                if (!await userManager.IsInRoleAsync(user, "admin"))
                {
                    await userManager.AddToRoleAsync(user, "admin");
                }
            }
        }
    }
}
