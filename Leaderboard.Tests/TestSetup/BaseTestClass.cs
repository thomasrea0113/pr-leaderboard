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
    public class BaseTestClass : IClassFixture<WebOverrideFactory>
    {
        protected WebOverrideFactory Factory { get; }
        protected HttpClient Client { get; }
        protected IServiceProvider Services { get; }

        public BaseTestClass(WebOverrideFactory factory)
        {
            Factory = factory;
        }

        protected IServiceScope CreateScope(out IServiceProvider provider)
        {
            var scope = Factory.Services.CreateScope();
            provider = scope.ServiceProvider;
            return scope;
        }

        protected IServiceScope CreateScope() => Factory.Services.CreateScope();
    }
}