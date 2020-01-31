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

            if (!_packedFiles.TryGetValue(Src, out var hashSrc))
                throw new ArgumentException($"source {Src} does not exist in the pack directory");

            var moduleName = Path.GetFileNameWithoutExtension(Src);
            
            var props = "[]";
            if (Props != null)
                props = JsonSerializer.Serialize(Props, Props.GetType());

            output.PreElement.AppendHtml($"<script src=\"{hashSrc}\"></script>");

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "script";
            output.Content.AppendHtml($@"ReactDOM.render(React.createElement(Components.{moduleName}, {props}), document.getElementById('{ElementId}'));");
        }
    }
}