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
    public class ReactComponentTagHelper : ReactBundleTagHelper
    {
        public object Props { get; set; }
        public string ElementId { get; set; }

        public ReactComponentTagHelper(IWebHostEnvironment env, ISpaStaticFileProvider spaFiles) : base(env, spaFiles)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Path.GetExtension(Src) != ".js")
                throw new ArgumentException($"source {Src} must be a javascript spa bundle");

            var assetPath = _webpackStats.GetAssetPath(Src);
            var asset = _allAssets.SingleOrDefault(a => a.Path == assetPath);
            if (asset == default)
                throw new ArgumentException($"source {assetPath} does not exist in the pack directory");

            var publicPath = _webpackStats.GetAssetPublicPath(Src);
            
            var props = "[]";
            if (Props != null)
                props = JsonSerializer.Serialize(Props, Props.GetType());

            output.PreElement.AppendHtml($"<script src=\"{publicPath}\"></script>");

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "script";
            output.Content.AppendHtml($@"ReactDOM.render(React.createElement(Components.{asset.Name}, {props}), document.getElementById('{ElementId}'));");
        }
    }
}