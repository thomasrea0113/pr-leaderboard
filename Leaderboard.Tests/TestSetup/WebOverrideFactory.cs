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
                services.AddSingleton<HttpContextAccessor>();

                var accessor = services.Single(s => s.ServiceType == typeof(IHttpContextAccessor));
                var fakeAccessor = new ServiceDescriptor(accessor.ServiceType, typeof(FakeHttpContextAccess), accessor.Lifetime);
                services.Replace(fakeAccessor);

                // replacing the DbContext will a test version, which includes a few exact debug options
                var context = services.Single(s => s.ServiceType == typeof(ApplicationDbContext));
                var testContext = new ServiceDescriptor(context.ServiceType, typeof(TestDbContext), context.Lifetime);
                services.Replace(testContext);
            })
                .ConfigureAppConfiguration(c => c.SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.unittest.json", false));
        }
    }
}