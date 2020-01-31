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
    public class ReactComponentTagHelper : TagHelper
    {
        public string Src { get; set; }
        public string ElementId { get; set; }
        public object Props { get; set; }
        public object Model { get; set; }

        private readonly List<IFileInfo> _packedFiles;
        private readonly string _spaDir;
        private readonly bool _IsDevelopment;

        public ReactComponentTagHelper(IWebHostEnvironment env, ISpaStaticFileProvider spaFiles)
        {
            _IsDevelopment = env.EnvironmentName == "Development";
            var provider = spaFiles.FileProvider
                    ?? throw new ArgumentNullException("Application is running is a production configuration, so the react development server will not be used. However, the build directory does not exist. Did you run 'npm run build' first?");
            _spaDir = provider.GetFileInfo("./").PhysicalPath;
            _packedFiles = RecursiveGetDirectoryContents(provider).ToList();
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

        private IFileInfo GetPackedFile(string path)
        {
            var dir = (Path.TrimEndingDirectorySeparator(_spaDir) + Path.GetDirectoryName(path)).Replace("\\", "\\\\");
            var fileName = Path.GetFileNameWithoutExtension(path);
            var ext = Path.GetExtension(path);

            // bundles should always match the <component-name>.chunk.<ext> pattern.
            var parts = fileName.Split('.').ToList();
            var chunkIndex = parts.LastIndexOf("chunk");

            string matchPattern;

            if (chunkIndex != -1)
                matchPattern = $"^{dir}\\\\{String.Join(',', parts.Take(chunkIndex))}.[0-9a-f]+.chunk{ext}$";
            else
                matchPattern = $"^{dir}\\\\{fileName}.[0-9a-f]+{ext}$";

            var hashMatch = new Regex(matchPattern);
            return _packedFiles
                .Single(f => hashMatch.IsMatch(f.PhysicalPath));
        }

        private string GetHashedPath(string path)
            => GetPackedFile(path).PhysicalPath
                .Replace(_spaDir, "/")
                .Replace("\\", "/");

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var hashPath = GetHashedPath(Src);

            var moduleName = Path.GetFileNameWithoutExtension(Src);

            var props = Props ?? Model ?? new { };
            output.PreElement.AppendHtml($"<script src=\"{hashPath}\"></script>");

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "script";
            output.Content.AppendHtml($@"ReactDOM.render(React.createElement(Components.{moduleName}, {JsonSerializer.Serialize(props, props.GetType())}), document.getElementById('{ElementId}'));");
        }
    }
}