using System;
using Arch.EntityFrameworkCore.UnitOfWork;
using Leaderboard.Areas.Profiles.data;
using Leaderboard.Areas.Profiles.Models;
using Leaderboard.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Leaderboard.Areas.Profiles.IdentityHostingStartup))]
namespace Leaderboard.Areas.Profiles
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddCustomRepository<UserProfileModel, ProfileRepository>();
            });
        }
    }
}