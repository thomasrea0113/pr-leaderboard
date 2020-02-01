using AutoFixture.Xunit2;

namespace Leaderboard.Tests.TestSetup.Fixtures
{
    public class DefaultDataAttribute : AutoDataAttribute
    {
        public DefaultDataAttribute() : base(() => new DefaultFixture())
        {
        }
    }
}