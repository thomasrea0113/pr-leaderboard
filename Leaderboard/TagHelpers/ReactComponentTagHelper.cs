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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Leaderboard.TagHelpers
{
    public class ReactComponentTagHelper : TagHelper
    {
        public string ComponentName { get; set; }
        public object Props { get; set; }

        /// <summary>
        /// Defaults to the ReactRootTagHelper DOM Id
        /// </summary>
        /// <value></value>
        public string ElementId { get; set; } = ReactRootTagHelper.ReactRootDOMId;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ComponentName == null) throw new ArgumentNullException(nameof(ComponentName));

            var props = "{}";
            if (Props != null)
                props = JsonConvert.SerializeObject(Props, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "script";
            output.Content.AppendHtml($@"ReactDOM.render(React.createElement(Components['{ComponentName}'], {props}), document.getElementById('{ElementId}'));");
        }
    }
}