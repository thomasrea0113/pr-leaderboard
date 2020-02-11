using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Leaderboard.Data;
using Leaderboard.Tests.TestSetup.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Leaderboard.Tests.TestSetup
{
    public class BaseTestClass : IClassFixture<WebOverrideFactory>, IDisposable
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

        public async Task WithScopeAsync(Func<IServiceProvider, Task> testAsync)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                await testAsync(scope.ServiceProvider);
            }
        }

        public void Dispose()
        {
            _factory.Dispose();
        }
    }
}