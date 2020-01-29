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
        public object Props { get; set; } = new {};

        private readonly List<IFileInfo> _packedFiles;
        private readonly string _spaDir;
        private readonly bool _isDevelopment;

        public ReactComponentTagHelper(IWebHostEnvironment env, ISpaStaticFileProvider spaFiles)
        {
            _isDevelopment = env.EnvironmentName == "Development";
            if (!_isDevelopment)
            {
                _spaDir = spaFiles.FileProvider.GetFileInfo("./").PhysicalPath;
                _packedFiles = RecursiveGetDirectoryContents(spaFiles.FileProvider).ToList();
            }
        }

        private IEnumerable<IFileInfo> RecursiveGetDirectoryContents(IFileProvider provider)
        {
            foreach(var file in provider.GetDirectoryContents("./"))
                if (!file.IsDirectory)
                    yield return file;
                else {
                    var providerTypeInstance = (IFileProvider)Activator.CreateInstance(provider.GetType(), file.PhysicalPath);
                    foreach (var file2 in RecursiveGetDirectoryContents(providerTypeInstance))
                        yield return file2;
                }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!_isDevelopment)
            {
                // TODO implement getting the chunk hash for each build using _packedFiles
                throw new NotImplementedException();
            }
            else{
                
                output.PreElement.AppendHtml($@"<script>
    var props = {JsonSerializer.Serialize(Props, Props.GetType())};
    var rootId = '{ElementId}';
</script>");
            }

            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("src", Src);
            output.TagName = "script";
        }
    }
}