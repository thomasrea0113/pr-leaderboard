using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Leaderboard.Services
{
    public interface IPartialRenderer
    {
        Task<string> RenderPartialToStringAsync<TModel>(string partialName, TModel model, object additionalViewData = null, HtmlHelperOptions options = null);
    }

    public class PartialRenderer : IPartialRenderer
    {
        private readonly IViewContextGenerator _generator;

        public PartialRenderer(IViewContextGenerator generator)
        {
            _generator = generator;
        }

        public async Task<string> RenderPartialToStringAsync<TModel>(string partialName, TModel model, object additionalViewData = null, HtmlHelperOptions options = null)
        {
            using var writer = new StringWriter();
            var context = _generator.GenerateViewContext(partialName, model, writer, additionalViewData, options);
            await context.View.RenderAsync(context).ConfigureAwait(false);
            return writer.GetStringBuilder().ToString();
        }
    }
}