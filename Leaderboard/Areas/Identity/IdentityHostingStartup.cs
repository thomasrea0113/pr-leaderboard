using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Leaderboard.Areas.Identity.IdentityHostingStartup))]
namespace Leaderboard.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}