using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Leaderboard.Areas.Profiles.IdentityHostingStartup))]
namespace Leaderboard.Areas.Profiles
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {

            });
        }
    }
}