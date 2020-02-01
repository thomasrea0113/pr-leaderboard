using System.Linq;
using Leaderboard.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Leaderboard.Tests.TestSetup
{
    public class WebOverrideFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var dbContextType = typeof(ApplicationDbContext);
            builder.ConfigureServices(services =>
            {
                // remove the existing db context (should be only one)
                services.Remove(services.Single(s => s.ServiceType == dbContextType));

                // readd the context as an in-memory database
                services.AddDbContext<ApplicationDbContext>(cnf =>
                {
                    cnf.UseInMemoryDatabase("test-db");
                });
            })
            .ConfigureAppConfiguration(c =>
                // without overriding base path, we'd still be point to the
                // TaskManager app bin directory, not TaskManager.Tests
                c.SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.unittest.json"));
        }
    }
}