using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Leaderboard.Tests.TestSetup.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.TestSetup
{
    public class BaseTestClass : IClassFixture<WebOverrideFactory>
    {
        protected WebOverrideFactory _factory { get; }
        protected HttpClient _client { get; }
        protected IServiceProvider _services { get; }

        public BaseTestClass(WebOverrideFactory factory)
        {
            _factory = factory;
        }

        public void WithScope(Action<IServiceScope> test)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                test(scope);
            }
        }

        public async Task WithScopeAsync(Func<IServiceScope, Task> testAsync)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                await testAsync(scope);
            }
        }
    }
}