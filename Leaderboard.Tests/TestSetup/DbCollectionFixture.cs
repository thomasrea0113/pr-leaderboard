using System;
using Xunit;

namespace Leaderboard.Tests.TestSetup
{

    [CollectionDefinition("seed")]
    public class DbCollectionFixture : ICollectionFixture<WebOverrideFactory>
    {
    }
}
