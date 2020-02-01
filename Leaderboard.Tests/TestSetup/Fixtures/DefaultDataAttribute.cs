using AutoFixture.Xunit2;

namespace Leaderboard.Tests.TestSetup.Fixtures
{
    public class DefaultDataAttribute : AutoDataAttribute
    {
        public DefaultDataAttribute(string dbName = default) : base(() => new DefaultFixture(dbName))
        {
        }
    }
}