using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Leaderboard.TagHelpers
{
    public class ReactComponentTagHelper : TagHelper
    {
        const string packPath = "/pack/static/js";

        private readonly IDirectoryContents _packedFiles;
        private readonly Regex regex = new Regex("", RegexOptions.IgnoreCase);
        public ReactComponentTagHelper(IWebHostEnvironment env)
        {
            _packedFiles = env.WebRootFileProvider.GetDirectoryContents(packPath);
        }

        public string Component { get; set; }
        public string ElementId { get; set; }

        public void AppendScript(TagHelperContent element, string pattern, bool inline = false)
        {
            var componentRegex = new Regex(pattern, RegexOptions.IgnoreCase);
            var file = _packedFiles.Single(f => componentRegex.IsMatch(f.Name));
            if (inline)
            {
                var content = new StreamReader(file.CreateReadStream()).ReadToEnd();
                element.AppendHtml($"<script>\n{content}\n</script>\n");
            }
            else
            {
                var webPath = Path.Join(packPath, file.Name).Replace("\\", "/");
                element.AppendFormat("<script src='{0}'></script>\n", webPath);
            }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            AppendScript(output.PreElement, $"^runtime~{Component}\\.[a-f0-9]{{8}}\\.js$", true);
            AppendScript(output.PreElement, $"^{2}\\.[a-f0-9]{{8}}\\.chunk\\.js$");
            AppendScript(output.PreElement, $"^{Component}\\.[a-f0-9]{{8}}\\.chunk\\.js$");

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "script";
            output.Content.AppendHtml($"ReactDOM.render(React.createElement({Component}.default), document.getElementById('{ElementId}'));");
        }
    }
}