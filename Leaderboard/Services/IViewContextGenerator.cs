using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Leaderboard.Services
{
    public interface IViewContextGenerator
    {
        ViewContext GenerateViewContext<TModel>(
            string viewName = default, TModel model = default, TextWriter writer = default,
            object additionalViewData = default, HtmlHelperOptions options = default);
    }

    public class ViewContextGenerator : IViewContextGenerator
    {
        private class FakeView : IView
        {
            public string Path => "View";

            public Task RenderAsync(ViewContext context)
            {
                return Task.FromResult(0);
            }
        }

        private readonly IRazorViewEngine _viewEngine;
        private readonly IModelMetadataProvider _metadataProvider;
        private readonly ActionContext _context;
        private readonly ITempDataDictionary _tempData;

        public ViewContextGenerator(
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

        public ViewContext GenerateViewContext<TModel>(
            string viewName = default, TModel model = default, TextWriter writer = default,
            object additionalViewData = default, HtmlHelperOptions options = default)
        {
            writer ??= new StringWriter();

            var viewData = new ViewDataDictionary(_metadataProvider, _context.ModelState)
            {
                Model = model
            };

            // if additional view data was provided, add each property
            if (additionalViewData != default)
            {
                foreach (var prop in additionalViewData.GetType().GetProperties())
                    viewData[prop.Name] = prop.GetValue(additionalViewData);
            }



            return new ViewContext(
                _context,
                viewName != default ? FindView(viewName) : new FakeView(),
                viewData,
                _tempData,
                writer,
                options ?? new HtmlHelperOptions()
            );
        }

        private IView FindView(string partialName)
        {
            var result = _viewEngine.GetView(null, partialName, false);
            if (result.Success)
                return result.View;

            result = _viewEngine.FindView(_context, partialName, false);
            if (result.Success)
                return result.View;

            var searchedLocations = String.Join(Environment.NewLine, result.SearchedLocations.Concat(result.SearchedLocations));
            throw new InvalidOperationException($"Unable to find partial '{partialName}'. The following locations were searched: {searchedLocations}");
        }
    }
}
