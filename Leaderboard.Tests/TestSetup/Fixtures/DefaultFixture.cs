using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Leaderboard.Tests.TestSetup.Fixtures
{
    public class AsUsernameAttribute : Attribute { }

    public class DefaultFixture : Fixture
    {
        public DefaultFixture(string dbName = default)
        {
            Customizations.Add(new StringFormatSpecimentBuilder<AsUsernameAttribute>("User{0}"));
        }
    }

    public class StringFormatSpecimentBuilder<TAttribute> : ISpecimenBuilder
        where TAttribute : Attribute
    {
        private readonly string _format;

        public StringFormatSpecimentBuilder(string _format)
        {
            if (!_format.Contains("{0}"))
                throw new ArgumentException();

            this._format = _format;
        }

        private string CreateFixture(ISpecimenContext context)
            => String.Format(_format, context.Create<string>());

        public object Create(object request, ISpecimenContext context)
        {
            if (request is Type t)
            {
                if (t == typeof(string))
                {
                    if (t.CustomAttributes.OfType<TAttribute>().Any())
                        return CreateFixture(context);
                }
                else if (typeof(IEnumerable<string>).IsAssignableFrom(t))
                {
                    return new string[] {
                        CreateFixture(context),
                        CreateFixture(context),
                        CreateFixture(context)
                    };
                }
            }
            return new NoSpecimen();
        }
    }
}