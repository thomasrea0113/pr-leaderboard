using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Leaderboard.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Leaderboard.Areas.Identity.Validators;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Data.SeedExtensions;
using System.Threading.Tasks;
using System;

namespace Leaderboard
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseLazyLoadingProxies()
                    // .EnableSensitiveDataLogging()
                    .UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            // adding the default user models
            services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;

                // disabling here so that we can handle email ourselves
                options.User.RequireUniqueEmail = false;

                options.Password.RequireNonAlphanumeric = false;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
                .AddRoles<ApplicationRole>()
                .AddUserManager<AppUserManager>()
                .AddRoleManager<AppRoleManager>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddUserValidator<EmailNotRequiredValidator>();

            services.AddScoped<AppUserManager>();

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSpaStaticFiles(cnf =>
            {
                cnf.RootPath = "../ClientApp/dist";
            });

            services.AddScoped<IServiceCollection>(i => services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config,
            ApplicationDbContext context, AppUserManager userManager, AppRoleManager roleManager)
        {
            // if seeding is configured for this configuration, attemps to run the seed method syncronously
            // and cancels it if it does not complete in time
            if (config.GetValue("SeedData:Enabled", false))
            {
                var timeout = config.GetValue("SeedData:TimeoutInSeconds", 60) * 1000;
                // Is this a bad idea? Seeding the data and continuing execution shouldn't cause issues.
                if (!app.SeedDevelopmentDataAsync(context, userManager, roleManager).Wait(timeout))
                    throw new TimeoutException($"The {nameof(SeedExtensions.SeedDevelopmentDataAsync)} method did not complete in {timeout} seconds");
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
