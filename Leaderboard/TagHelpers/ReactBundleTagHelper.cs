using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.SpaServices.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Leaderboard.TagHelpers
{
    public class ReactBundleTagHelper : TagHelper
    {
        public string Src { get; set; }

        protected readonly Dictionary<string, string> _packedFiles;
        private readonly string _spaDir;

        public ReactBundleTagHelper(IWebHostEnvironment env, ISpaStaticFileProvider spaFiles)
        {
            var provider = spaFiles.FileProvider
                    ?? throw new ArgumentNullException("Application is running is a production configuration, so the react development server will not be used. However, the build directory does not exist. Did you run 'npm run build' first?");
            _spaDir = provider.GetFileInfo("./").PhysicalPath;
            var files = RecursiveGetDirectoryContents(provider).ToList();

            var hashRegex = new Regex("\\.[0-9a-f]{20}(?=\\.[^\\.]+$)");

            _packedFiles = files.Select(f => {
                var path = f.PhysicalPath.Replace(_spaDir, "/");
                if (Path.DirectorySeparatorChar == '\\')
                    path = path.Replace('\\', '/');
                return path;
            }).ToDictionary(f => hashRegex.Replace(f, ""), f => f);
        }

        private IEnumerable<IFileInfo> RecursiveGetDirectoryContents(IFileProvider provider)
        {
            foreach (var file in provider.GetDirectoryContents("./"))
                if (!file.IsDirectory)
                    yield return file;
                else
                {
                    var providerTypeInstance = (IFileProvider)Activator.CreateInstance(provider.GetType(), file.PhysicalPath);
                    foreach (var file2 in RecursiveGetDirectoryContents(providerTypeInstance))
                        yield return file2;
                }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var isJs = Path.GetExtension(Src) == ".js";

            if (!_packedFiles.TryGetValue(Src, out var hashSrc))
                throw new ArgumentException($"source {Src} does not exist in the pack directory");

            if (isJs)
            {
                output.TagMode = TagMode.StartTagAndEndTag;
                output.TagName = "script";
                output.Attributes.Add("src", hashSrc);
            }
            else
            {
                output.TagMode = TagMode.StartTagOnly;
                output.TagName = "link";
                output.Attributes.Add("rel", "stylesheet");
                output.Attributes.Add("type", "text/css");
                output.Attributes.Add("href", hashSrc);
            }
        }
    }
}