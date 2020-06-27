using System;
using Microsoft.AspNetCore.Razor.TagHelpers;
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