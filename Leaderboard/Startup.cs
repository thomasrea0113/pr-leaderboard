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
using Leaderboard.Areas.Identity;
using Leaderboard.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Leaderboard.Routing.Constraints;
using Newtonsoft.Json.Converters;

namespace Leaderboard
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _envName = env.EnvironmentName;
        }

        public IConfiguration Configuration { get; }

        private readonly string _envName;

        private readonly ILoggerFactory Factory = LoggerFactory.Create(builder => builder.AddDebug());

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppConfiguration>(Configuration.GetSection("AppSettings"));

            // FIXME need to seed the database Before the database context is available for injection
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));

                if (_envName != "Production")
                    options.EnableDetailedErrors()
                        .UseLoggerFactory(Factory)
                        .EnableSensitiveDataLogging();
            });

            services.AddScoped<DbContext>(services => services.GetRequiredService<ApplicationDbContext>());

            // adding the default user models
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;

                // disabling here so that we can handle email ourselves
                options.User.RequireUniqueEmail = false;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";

                options.Password.RequireNonAlphanumeric = false;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
                .AddUserManager<AppUserManager>()
                .AddRoleManager<AppRoleManager>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserStore<AppUserStore>()
                .AddRoleStore<AppRoleStore>()
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                // for adding additional user claims
                .AddClaimsPrincipalFactory<ApplicationClaimsPrincipalFactory>()
                .AddUserValidator<EmailNotRequiredValidator>()
                .AddUserValidator<UserNameValidator>();

            // for persisting user messages across requests
            services.AddScoped<IMessageQueue, TempDataMessageQueue>();

            services.AddScoped<IEmailSender, SmtpEmailSender>();

            // for rendering partial views in emails. PartialRenderer depends on
            // ActionContextAccessor
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IViewContextGenerator, ViewContextGenerator>();
            services.AddTransient<IPartialRenderer, PartialRenderer>();

            services.AddRouting(o =>
            {
                // route constraints - used to make assertions about route parameters
                o.ConstraintMap["slug"] = typeof(SlugConstraint);
                o.ConstraintMap["range"] = typeof(RangeConstraint);
                o.ConstraintMap["gender"] = typeof(GenderConstraint);
            });

            services.AddAntiforgery(o =>
            {
                // this should make the csrf token available in the cookies, but it
                // was inconsistent and seemed to be bugged. We'll use the
                // custom Razor Page convention below instead.
                o.Cookie.HttpOnly = false;
                o.Cookie.Name = "csrfToken";

                // This should be set explicitly so that we can use it dynamically in
                // our react components.
                o.FormFieldName = "__RequestVerificationToken";
                o.HeaderName = "X-CSRF-TOKEN";
            });

            services.AddRazorPages(o =>
            {
                o.Conventions.AuthorizeAreaFolder("Admin", "/", "AppAdmin");
                // o.Conventions.AddAreaFolderApplicationModelConvention("Admin", "/", a =>
                //     a.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder()
                //         .RequireRole("Admin")
                //         .Build())));
            }).AddNewtonsoftJson(o =>
            {
                o.UseCamelCasing(true);
                o.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            services.AddAuthorization(o =>
            {
                // It's important that the role name be all lowercase
                o.AddPolicy("AppAdmin", p => p.RequireRole("admin"));
            });

            // controllers will be used for api CRUD actions
            services.AddControllers().AddControllersAsServices();

            services.AddSpaStaticFiles(cnf =>
            {
                cnf.RootPath = "../ClientApp/dist";
            });

            services.AddScoped<IFormFieldAttributeProvider, FormFieldAttributeProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
                endpoints.MapRazorPages();
                endpoints.MapControllers();

                // The Admin area is a react component that uses ReactRouter to
                // handle routing inside the component, so all pages
                // should route to the Index page
                endpoints.MapFallbackToAreaPage("/Index", "Admin");
            });
        }
    }
}
