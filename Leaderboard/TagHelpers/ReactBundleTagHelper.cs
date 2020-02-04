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
    public class WebpackStats {
        public string publicPath { get; set; }
        public string Hash { get; set; }
        public Dictionary<string, string[]> AssetsByChunkName { get; set; }

        public string GetAssetPath(string pathPattern)
            => pathPattern.Replace("[hash]", Hash, true, null);
            // TODO implement chunk support

        public string GetAssetPublicPath(string pathPattern)
            => String.Join("/", publicPath, GetAssetPath(pathPattern));
    }

    public class Asset {
        public string Name { get; private set; }
        public string Path { get; private set; }

        public ValueTuple<string, string> deconstruct() => (Name, Path);
        public static implicit operator Asset(ValueTuple<string, string> tuple)
            => new Asset {
                Name = tuple.Item1,
                Path = tuple.Item2
            };
    }

    public class ReactBundleTagHelper : TagHelper
    {
        public string Src { get; set; }

        protected readonly WebpackStats _webpackStats;
        protected readonly List<Asset> _allAssets;

        public ReactBundleTagHelper(IWebHostEnvironment env, ISpaStaticFileProvider spaFiles)
        {
            var provider = spaFiles.FileProvider
                    ?? throw new ArgumentNullException($"The SPA directory does not exist. Did you run 'npm run build' first?");
            
            var statsFile = provider.GetFileInfo("stats.json");

            if (!statsFile.Exists)
                throw new InvalidOperationException("stats.json does not exist at the root of the spa static files directory. Is the webpack-stats-plugin plugin properly configured?");
            
            var statsFileContents = new StreamReader(statsFile.CreateReadStream()).ReadToEnd();
            _webpackStats = (WebpackStats)JsonSerializer.Deserialize(statsFileContents,
                typeof(WebpackStats),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            // flatten the chunks to one list of Assets
            _allAssets = _webpackStats.AssetsByChunkName
                .SelectMany(a => a.Value.Select(a2 => (Asset)(a.Key, a2)))
                .ToList();
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var isJs = Path.GetExtension(Src) == ".js";

            var assetPath = _webpackStats.GetAssetPath(Src);
            if (!_allAssets.Any(ap => ap.Path == assetPath))
                throw new ArgumentException($"source {assetPath} does not exist in the pack directory.");

            var publicPath = _webpackStats.GetAssetPublicPath(Src);

            if (isJs)
            {
                output.TagMode = TagMode.StartTagAndEndTag;
                output.TagName = "script";
                output.Attributes.Add("src", publicPath);
            }
            else
            {
                output.TagMode = TagMode.StartTagOnly;
                output.TagName = "link";
                output.Attributes.Add("rel", "stylesheet");
                output.Attributes.Add("type", "text/css");
                output.Attributes.Add("href", publicPath);
            }
        }
    }
}