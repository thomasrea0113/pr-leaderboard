using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Leaderboard.Areas.Uploads.Models;
using Leaderboard.Areas.Uploads.Services;
using Leaderboard.Data;
using Leaderboard.Tests.TestSetup;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests
{
    public class FileUploadTests : BaseTestClass
    {
        public FileUploadTests(WebOverrideFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Test1()
        {
            using var _ = CreateScope(out var scope);
            var files = scope.GetRequiredService<ICreatableFileProvider>();

            using var client = Factory.CreateClient();

            var content = new MultipartFormDataContent();
            var bytes = await File.ReadAllBytesAsync("TestFiles/ipsum.txt");
            content.Add(new ByteArrayContent(bytes), nameof(FileUploadModel.File));

            await client.PostAsync("Upload", content);
        }
    }
}