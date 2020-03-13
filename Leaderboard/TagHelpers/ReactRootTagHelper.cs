using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Leaderboard.TagHelpers
{

    [HtmlTargetElement("react-root")]
    public class ReactRootTagHelper : TagHelper
    {
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public const string ReactRootDOMId = "react-root";
        public const string ReactRootViewDataKey = "ReactRootDeclared";
        public bool ReactRootDeclared
        {
            get => Convert.ToBoolean(ViewContext.ViewData[ReactRootViewDataKey]);
            set => ViewContext.ViewData[ReactRootViewDataKey] = value;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";

            if (!ReactRootDeclared)
            {
                output.Attributes.Add("id", ReactRootDOMId);
                ReactRootDeclared = true;
            }
        }
    }
}