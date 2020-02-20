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
        private readonly IRazorViewEngine _viewEngine;
        private readonly IModelMetadataProvider _metadataProvider;
        private readonly ActionContext _context;
        private readonly ITempDataDictionary _tempData;

        public PartialRenderer(
            IRazorViewEngine viewEngine,
            IActionContextAccessor _accessor,
            IModelMetadataProvider metadataProvider,
            ITempDataDictionaryFactory _factory)
        {
            _viewEngine = viewEngine;
            _metadataProvider = metadataProvider;
            _context = _accessor.ActionContext;
            _tempData = _factory.GetTempData(_context.HttpContext);
        }

        public async Task<string> RenderPartialToStringAsync<TModel>(string partialName, TModel model, object additionalViewData = null, HtmlHelperOptions options = null)
        {
            var partial = FindView(_context, partialName);

            var writer = new StringWriter();

            var viewData = new ViewDataDictionary(_metadataProvider, _context.ModelState)
            {
                Model = model
            };

            // if additional view data was provided, add each property
            if(additionalViewData != default)
            {
                foreach (var prop in additionalViewData.GetType().GetProperties())
                    viewData[prop.Name] = prop.GetValue(additionalViewData);
            }

            var _viewContext = new ViewContext(
                _context,
                partial,
                viewData,
                _tempData,
                writer,
                options ?? new HtmlHelperOptions()
            );

            await partial.RenderAsync(_viewContext);
            return writer.GetStringBuilder().ToString();
        }

        private IView FindView(ActionContext actionContext, string partialName)
        {
            var result = _viewEngine.GetView(null, partialName, false);
            if (result.Success)
                return result.View;

            result = _viewEngine.FindView(actionContext, partialName, false);
            if (result.Success)
                return result.View;

            var searchedLocations = String.Join(Environment.NewLine, result.SearchedLocations.Concat(result.SearchedLocations));
            throw new InvalidOperationException($"Unable to find partial '{partialName}'. The following locations were searched: {searchedLocations}");
        }
    }
}