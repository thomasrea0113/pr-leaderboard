using System.Linq;
using Leaderboard.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Hosting;

namespace Leaderboard.Tests.TestSetup
{
    /// <summary>
    /// A web application factory that provides an InMemory database for testing. The constructor
    /// takes an options DB Name, so that tests can share data if necessary
    /// </summary>
    public class WebOverrideFactory : WebApplicationFactory<Startup>
    {
        private readonly string _dbName;

        public WebOverrideFactory(string dbName = default)
        {
            _dbName = dbName ?? Guid.NewGuid().ToString();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var dbContextType = typeof(DbContextOptions<ApplicationDbContext>);
            builder.ConfigureServices(services =>
            {
                // remove the existing db context (should be only one)
                services.Remove(services.Single(s => s.ServiceType == dbContextType));

                // readd the context as an in-memory database
                services.AddDbContext<ApplicationDbContext>(cnf =>
                {
                    // lazy loading prevents us from having to expliclity load all of our related models
                    cnf.UseLazyLoadingProxies()
                        .UseInMemoryDatabase(_dbName);
                });
            })
            .ConfigureAppConfiguration(c =>
                // without overriding base path, we'd still be pointing to the Leaderboard
                // bin directory, not the Leaderboard.Tests bin directory
                c.SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.unittest.json"));
        }
    }
}