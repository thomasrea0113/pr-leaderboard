using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Leaderboard.Data;
using Leaderboard.Tests.TestSetup.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Xunit;

namespace Leaderboard.Tests.TestSetup
{
    [Collection("seed")]
    public class BaseTestClass
    {
        protected WebOverrideFactory Factory { get; }
        protected HttpClient Client { get; }
        protected IServiceProvider Services { get; }

        public BaseTestClass(WebOverrideFactory factory)
        {
            Factory = factory;

            // We need to wait for the server to be ready,
            // so we send a request to the root page
            using var client = factory.CreateClient();
        }

        protected IServiceScope CreateScope(out IServiceProvider provider)
        {
            var scope = Factory.Services.CreateScope();
            provider = scope.ServiceProvider;
            return scope;
        }
    }
}