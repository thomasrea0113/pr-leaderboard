using System;
using System.IO;
using System.Threading.Tasks;
using Leaderboard.Areas.Uploads.Exceptions;
using Leaderboard.Areas.Uploads.Models;
using Leaderboard.Areas.Uploads.Services;
using Leaderboard.Areas.Uploads.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using SampleApp.Models;

namespace SampleApp.Pages
{
    public class UploadModel : PageModel
    {
        private readonly ILogger<UploadModel> _logger;
        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMultipartModelBinder _mpBinder;
        private readonly ICreatableFileProvider _fileProvider;

        public UploadModel(ILogger<UploadModel> logger,
            DbContext context, UserManager<IdentityUser> userManager,
            IMultipartModelBinder mpBinder, ICreatableFileProvider fileProvider)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _mpBinder = mpBinder;
            _fileProvider = fileProvider;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            FileContent fileContent;
            FormValueProvider provider;

            var user = await _userManager.GetUserAsync(HttpContext.User);

            // stores all files uploaded by a given user in the same directory
            var streamFactory = _fileProvider.GetWriteStreamFactory(user.UserName);

            try
            {
                (fileContent, provider) = await _mpBinder.ProcessMultipartRequestAsync(Request, streamFactory);
            }
            catch (MultipartBindingException ex)
            {
                _logger.LogError(ex, "Error binding posted file");
                ModelState.TryAddModelException("", ex);
                throw;
            }

            var model = new FileUploadModel();
            await TryUpdateModelAsync(model, "", provider);

            // TODO save file to disk and update database
            var added = _context.Set<AppFile>().Add(new AppFile
            {
                Path = Path.Join(Path.GetRandomFileName(), model.File.Name),
                CreatedById = _userManager.GetUserId(HttpContext.User),
                Size = model.File.ContentStream.Length,
                UploadDate = DateTime.UtcNow,
            });

            await _context.SaveChangesAsync();

            return new CreatedResult(Url.Page(null), added.Entity);
        }
    }
}
