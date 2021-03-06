using System;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Leaderboard.Pages
{
    public class ErrorModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }

    public class ErrorPageModel : PageModel
    {
        private readonly ILogger<ErrorPageModel> _logger;

        public ErrorModel PageError { get; set; }

        public ErrorPageModel(ILogger<ErrorPageModel> logger)
        {
            _logger = logger;
        }

        private static string GetMessageForStatusCode(HttpStatusCode statusCode)
            => statusCode switch
            {
                HttpStatusCode.NotFound => "Not Found",
                _ => "",
            };

        public void OnGet()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            // in cases where the user routes directly to the error page
            if (Response.StatusCode > 199 && Response.StatusCode < 300)
                Response.StatusCode = 404;

            var statusCode = Response.StatusCode;

            if (exceptionHandlerPathFeature?.Error is Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An exception has ocurred at path '{path}' and was captured globably",
                    exceptionHandlerPathFeature.Path);
            }

            if (statusCodeReExecuteFeature != null)
                _logger.LogWarning(
                    "client got status code '{statusCode}' for address '{path}'",
                    statusCode,
                    statusCodeReExecuteFeature.OriginalPath);

            PageError = new ErrorModel
            {
                StatusCode = statusCode,
                Message = GetMessageForStatusCode((HttpStatusCode)statusCode)
            };
        }
    }
}