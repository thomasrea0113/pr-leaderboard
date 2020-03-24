using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Leaderboard.Areas.Uploads.Exceptions;
using Leaderboard.Areas.Uploads.Models;
using Leaderboard.Areas.Uploads.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using SampleApp.Utilities;
using static SampleApp.Utilities.FileHelpers;

namespace Leaderboard.Areas.Uploads.Utilities
{
    public class MultipartModelBinderConfig
    {
        public int MultipartBoundaryLengthLimit { get; set; }
        public long FileSizeLimit { get; set; }
        public int ValueCountLimit { get; set; }
        public string StoredFilesPath { get; set; }
    }

    public interface IMultipartModelBinder
    {
        Task<(FileContent, FormValueProvider)> ProcessMultipartRequestAsync(HttpRequest request, IWriteStreamFactory streamFactory, params string[] permittedExtensions);
    }

    public class MultipartModelBinder : IMultipartModelBinder
    {
        private readonly MultipartModelBinderConfig _defaultFormOptions;

        public MultipartModelBinder(IConfiguration config)
        {
            _defaultFormOptions = new MultipartModelBinderConfig();
            config.Bind(nameof(MultipartModelBinder), _defaultFormOptions);
        }

        public async Task<(FileContent, FormValueProvider)> ProcessMultipartRequestAsync(
            HttpRequest request,
            IWriteStreamFactory streamFactory,
            params string[] permittedExtensions)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(request.ContentType))
                throw new MultipartBindingException($"Content type must be multi-part");

            // Accumulate the form data key-value pairs in the request (formAccumulator).
            var formAccumulator = new KeyValueAccumulator();

            FileContent fileContent = null;

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, request.Body);

            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                var hasContentDispositionHeader =
                    ContentDispositionHeaderValue.TryParse(
                        section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper
                        .HasFileContentDisposition(contentDisposition))
                    {
                        fileContent = new FileContent(
                            contentDisposition.FileName.Value,
                            await ProcessStreamedFileAsync(streamFactory, section, contentDisposition,
                                permittedExtensions, _defaultFormOptions.FileSizeLimit));
                    }
                    else if (MultipartRequestHelper
                        .HasFormDataContentDisposition(contentDisposition))
                    {
                        // Don't limit the key name length because the 
                        // multipart headers length limit is already in effect.
                        var key = HeaderUtilities
                            .RemoveQuotes(contentDisposition.Name).Value;
                        var encoding = GetEncoding(section.ContentType);

                        if (encoding == null)
                            throw new MultipartBindingException("invalid encoding");

                        using var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true);

                        // The value length limit is enforced by 
                        // MultipartBodyLengthLimit
                        var value = await streamReader.ReadToEndAsync();

                        if (string.Equals(value, "undefined",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            value = string.Empty;
                        }

                        formAccumulator.Append(key, value);

                        if (formAccumulator.ValueCount >
                            _defaultFormOptions.ValueCountLimit)
                            throw new MultipartBindingException(
                                $"Form key count limit of {_defaultFormOptions.ValueCountLimit} is exceeded.");
                    }
                }

                // Drain any remaining section body that hasn't been consumed and
                // read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            // fileContent may be null if the form didn't have any file data

            return (fileContent, new FormValueProvider(
                BindingSource.Form,
                new FormCollection(formAccumulator.GetResults()),
                CultureInfo.CurrentCulture));
        }
    }
}
