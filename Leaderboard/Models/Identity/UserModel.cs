using System;
using System.Collections.Generic;
using Leaderboard.Models.Relationships;
using Microsoft.AspNetCore.Identity;

namespace Leaderboard.Models.Identity
{
    public class UserModel : IdentityUser<Guid>
    {
        public List<UserLeaderboard> UserLeaderboards { get; set; }
    }
}