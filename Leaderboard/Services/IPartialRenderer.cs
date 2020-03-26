using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

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
            var writer = new StringWriter();
            var context = _generator.GenerateViewContext(partialName, model, writer, additionalViewData, options);
            await context.View.RenderAsync(context);
            return writer.GetStringBuilder().ToString();
        }
    }
}