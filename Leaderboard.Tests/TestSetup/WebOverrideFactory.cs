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
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Leaderboard.Areas.Identity.Models;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Leaderboard.Tests.TestSetup
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppUserManager _userManager;

        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
            SignInManager<ApplicationUser> signInManager, AppUserManager userManager)
            : base(options, logger, encoder, clock)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var user = await _userManager.FindByNameAsync(authHeader.Parameter).ConfigureAwait(false);
            var principal = await _signInManager.ClaimsFactory.CreateAsync(user).ConfigureAwait(false);
            var ticket = new AuthenticationTicket(principal, "Test");
            return AuthenticateResult.Success(ticket);
        }
    }
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

                // for mocking authentication
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "Test", options => { });
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

        public new HttpClient CreateClient()
            => CreateClient(
        new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // with .net core 2.1, a generic IHost was added in favor over IWebHost.
        // for applications still using IWebHost, this function would be called instead
        // of the above.
        // protected override TestServer CreateServer(IWebHostBuilder builder);
    }
}