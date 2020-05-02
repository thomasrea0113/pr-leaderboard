using System;
using System.IO;
using Leaderboard.Areas.Uploads.Services;
using Leaderboard.Areas.Uploads.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Leaderboard.Areas.Uploads.Filters;
using Microsoft.Extensions.Options;

[assembly: HostingStartup(typeof(Leaderboard.Areas.Uploads.UploadsHostingStartup))]

namespace Leaderboard.Areas.Uploads
{
    public class UploadsHostingStartup : IHostingStartup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.PostConfigure<RazorPagesOptions>(options =>
            {
                options.Conventions
                    .AddAreaFolderApplicationModelConvention("Uploads", "/",
                        model =>
                        {
                            model.Filters.Add(
                                new DisableFormValueModelBindingAttribute());
                        });
            });

            services.AddSingleton<ICreatableFileProvider>(p =>
            {
                var configObject = p.GetRequiredService<IOptions<AppConfiguration>>().Value.MultipartModelBinder;

                var path = new Uri(configObject.StoredFilesPath, UriKind.Absolute);
                Directory.CreateDirectory(path.AbsolutePath);
                return new CreatablePhysicalFileProvider(path.AbsolutePath);
            });

            services.AddSingleton<IMultipartModelBinder, MultipartModelBinder>();
        }

        public void Configure(IWebHostBuilder builder) => builder.ConfigureServices(ConfigureServices);
    }
}
