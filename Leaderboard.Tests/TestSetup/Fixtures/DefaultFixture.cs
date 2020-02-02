using System;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Leaderboard.Tests.TestSetup.Fixtures
{
    public class DefaultFixture : Fixture
    {
        public DefaultFixture(string dbName = default)
        {
            // var overrides = new WebOverrideFactory(dbName);

            // this.Customize(WebApplicationSpecimenBuilder.Create(overrides).ToCustomization());

            // this.Inject<WebApplicationFactory<Startup>>(overrides);

            // this.Register(() => overrides.CreateClient());
        }
    }

    /// <summary>
    /// Convenient creator functions so that the TStartup generic argument is
    /// inferred, rather than having to provide it explicility on the constructor
    /// </summary>
    public static class WebApplicationSpecimenBuilder
    {
        public static WebApplicationSpecimenBuilder<TStartup> Create<TStartup>(WebApplicationFactory<TStartup> factory) where TStartup : class
            => new WebApplicationSpecimenBuilder<TStartup>(factory);
    }

    /// <summary>
    /// Funnels all AutoData objects through the Test Servers service collection
    /// </summary>
    /// <typeparam name="TStartup"></typeparam>
    public class WebApplicationSpecimenBuilder<TStartup> : ISpecimenBuilder, IDisposable
        where TStartup : class
    {
        private WebApplicationFactory<TStartup> _factory { get; }
        private IServiceScope _scope { get; }

        public WebApplicationSpecimenBuilder(WebApplicationFactory<TStartup> factory)
        {
            _factory = factory;
            var services = _factory.Services;
            _scope = _factory.Services.CreateScope();
        }
        
        public object Create(object request, ISpecimenContext context)
        {
            if (request is Type t)
            {
                var service = _scope.ServiceProvider.GetService(t);
                if (service == null)
                    return new NoSpecimen();
                return service;
            }
            return new NoSpecimen();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}