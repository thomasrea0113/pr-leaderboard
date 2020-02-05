using System.Linq;
using Leaderboard.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Hosting;
using Npgsql.Logging;

namespace Leaderboard.Tests.TestSetup
{
    /// <summary>
    /// A web application factory that provides an InMemory database for testing. The constructor
    /// takes an options DB Name, so that tests can share data if necessary
    /// </summary>
    public class WebOverrideFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var dbContextType = typeof(DbContextOptions<ApplicationDbContext>);
            builder.ConfigureServices(services =>
            {
                // configure services here
                var ctx = services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // if newer migrations have been created, they will be automatically applied here. The test database
                // uses a temporarly file system, so it will get wiped out after every time the container shuts down
                if (ctx.Database.GetPendingMigrations().Any())
                    ctx.Database.Migrate();
            })
            .ConfigureAppConfiguration(c =>
                // without overriding base path, we'd still be pointing to the Leaderboard
                // bin directory, not the Leaderboard.Tests bin directory
                c.SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.unittest.json"));
        }
    }
}