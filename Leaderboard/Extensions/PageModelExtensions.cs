using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Leaderboard.Extensions.PageModelExtensions
{
    public static class PageModelExtensions
    {
        /// <summary>
        /// JQuery automatically executes Javascript that is returned by an ajax
        /// request. This should be used in place of <see cref="PageModel.Redirect" />
        /// when a handler is intended to be called via jquery ajax.
        /// </summary>
        /// <param name="url">The unencoded redirect URL</param>
        /// <returns></returns>
        public static ContentResult JavascriptRedirect(this PageModel _, string url)
            => new ContentResult
            {
                ContentType = "application/x-javascript",
                Content = $"window.location.href = '{new HtmlString(url).Value}';"
            };
    }
}