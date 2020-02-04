using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Kernel;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Leaderboard.Tests.TestSetup.Fixtures
{
    public class AsUsernameAttribute : Attribute { }

    public class DefaultFixture : Fixture
    {
        public DefaultFixture(string dbName = default)
        {
            this.Register<TagModel>(() => new TagModel {
                Name = $"Tag {this.Create<string>()}"
            });

            this.Register<LeaderboardModel>(() => new LeaderboardModel {
                Name = $"Leaderboard {this.Create<string>()}"
            });

            this.Register<IdentityUser>(() => new IdentityUser {
                UserName = $"User_{this.Create<string>()}",
                Email = $"Email.{this.Create<string>()}@test.com"
            });
        }
    }
}