using Leaderboard.Tests.TestSetup.Fixtures;
using Xunit;

namespace Leaderboard.Tests.TestSetup
{
    public abstract class TestClass : IClassFixture<DefaultFixture>
    {
        protected DefaultFixture _Fixture { get; }
        
        public TestClass(DefaultFixture fixture)
        {
            _Fixture = fixture;
        }
    }
}