using System.Linq;
using Leaderboard.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Leaderboard.Extensions;
using System;
using Leaderboard.Areas.Identity.Managers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

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
            builder.ConfigureTestServices(services =>
            {
                // replacing the current HttpAccessor, so that we can inject a fake Context only if
                // the current context is null
                services.TryAddSingleton<HttpContextAccessor>();

                // disabling csrf to make testing a lot easier
                services.Configure<RazorPagesOptions>(options =>
                {
                    options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
                });

                var accessor = services.Single(s => s.ServiceType == typeof(IHttpContextAccessor));
                var fakeAccessor = new ServiceDescriptor(accessor.ServiceType, typeof(FakeHttpContextAccess), accessor.Lifetime);
                services.Replace(fakeAccessor);
            })
                .ConfigureAppConfiguration(c => c.SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.unittest.json", false));

            builder.UseEnvironment("Testing");
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var server = base.CreateHost(builder);
            using var scope = server.Services.CreateScope();
            var provider = scope.ServiceProvider;
            var env = provider.GetRequiredService<IWebHostEnvironment>().EnvironmentName;
            var adminUsers = provider.GetRequiredService<IOptionsSnapshot<AppConfiguration>>().Value.AdminUsers;

            if (!provider.MigrateAsync(env, true).Wait(8000))
                throw new TimeoutException("seed timeout");

            if (!provider.GetRequiredService<AppUserManager>()
                .EnsureAdminUsersAsync(adminUsers.ToArray())
                .Wait(8000))
                throw new TimeoutException("Ensure admin users timeout");
            return server;
        }

        // with .net core 2.1, a generic IHost was added in favor over IWebHost.
        // for applications still using IWebHost, this function would be called instead
        // of the above.
        // protected override TestServer CreateServer(IWebHostBuilder builder);
    }
}