using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Profiles.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Leaderboard.Areas.Profiles.DbContextExtensions
{
    public static class Extensions
    {
        public static async Task EnsureProfilesAsync(this DbContext ctx, IdentityUser[] users)
        {
            // TODO getting all the users prevents multiple DB calls, but could
            // be problematic with a large number of users
            var profileSet = ctx.Set<UserProfileModel>();
            var userProfiles = await profileSet.ToListAsync();

            // any time a user is created, make sure a profile for them also exists
            foreach (var user in users)
                if (!userProfiles.Any(p => p.UserId == user.Id))
                    await profileSet.AddAsync(new UserProfileModel { UserId = user.Id });
        }
    }
}