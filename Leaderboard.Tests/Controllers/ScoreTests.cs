using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Leaderboard.Areas.Leaderboards.Controllers;
using Leaderboard.Areas.Leaderboards.ViewModels;
using Leaderboard.Extensions;
using Leaderboard.Tests.TestSetup;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Xunit;

namespace Leaderboard.Tests.Controllers
{
    public class ScoreTests : BaseTestClass
    {
        public ScoreTests(WebOverrideFactory factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("lifterduder")]
        public async Task TestApprove(string user)
        {
            using var scope = Factory.Services.CreateScope();
            using var client = Factory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test", user);

            var resp = await client.GetAsync("/api/login").ConfigureAwait(false);

            var url = scope.ServiceProvider.GetRequiredService<LinkGenerator>();
            var context = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;

            var uri = url.GetPathByAction(
                context, nameof(ScoresController.Approve),
                ControllerExtensions.GetControllerName<ScoresController>());

            Assert.NotNull(uri);

            var jsonOptions = scope.ServiceProvider.GetRequiredService<IOptions<JsonSerializerSettings>>().Value;
            var serializer = JsonSerializer.Create(jsonOptions);

            var content = JsonContent.Create(new ApproveModel
            {
                Ids = new List<string>
                {
                    "1",
                    "2",
                    "3"
                }
            });

            var response = await client.PatchAsync(uri, content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var data = serializer.Deserialize<IEnumerable<ScoreViewModel>>(new JsonTextReader(new StreamReader(responseStream)));
        }

        [Theory]
        [InlineData("lifterduder")]
        public async Task TestAllScores(string user)
        {
            using var scope = Factory.Services.CreateScope();
            using var client = Factory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test", user);

            var url = scope.ServiceProvider.GetRequiredService<LinkGenerator>();
            var context = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;

            var uri = url.GetPathByAction(
                context, nameof(ScoresController.Get),
                ControllerExtensions.GetControllerName<ScoresController>(),
                new
                {
                    isApproved = true,
                    top = 10
                });

            Assert.NotNull(uri);

            var jsonOptions = scope.ServiceProvider.GetRequiredService<IOptions<JsonSerializerSettings>>().Value;
            var serializer = JsonSerializer.Create(jsonOptions);

            var responseStream = await client.GetStreamAsync(uri).ConfigureAwait(false);

            var data = serializer.Deserialize<IEnumerable<ScoreViewModel>>(new JsonTextReader(new StreamReader(responseStream)));
        }
    }
}