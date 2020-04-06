using System.Threading.Tasks;
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

            var services = host.Services;
            var config = services.GetRequiredService<IConfiguration>();
            var env = services.GetRequiredService<IWebHostEnvironment>().EnvironmentName;

            // TODO log seed, catch Migrate exception and notify user that no changes to the database were applied
            if (config.GetValue("AutoMigrate:Enabled", false))
            {
                using var scope = host.Services.CreateScope();
                await scope.ServiceProvider.MigrateAsync(env, config.GetValue("AutoMigrate:AutoSeed", false));
            }

            await host.RunAsync();
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
