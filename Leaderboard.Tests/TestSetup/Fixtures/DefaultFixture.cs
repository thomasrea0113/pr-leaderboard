using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Kernel;
using Leaderboard.Areas.Identity.Models;
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
        public DefaultFixture()
        {
            this.Register<Division>(() => new Division {
                Name = $"Tag {this.Create<string>()}"
            });

            this.Register<LeaderboardModel>(() => new LeaderboardModel {
                Name = $"Leaderboard {this.Create<string>()}"
            });

            this.Register<ApplicationUser>(() => new ApplicationUser {
                UserName = $"User_{this.Create<string>()}",
                Email = $"Email.{this.Create<string>()}@test.com"
            });
        }
    }
}