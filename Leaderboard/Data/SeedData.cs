using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Data.Extensions;
using Leaderboard.Models.Relationships;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Data.SeedExtensions
{
    public static class SeedExtensions
    {
        /// <summary>
        /// Allows for identity user/role seeding using the provided managers.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<IApplicationBuilder> SeedIdentityAsync(this IApplicationBuilder builder, DbContext context,
        AppUserManager userManager, AppRoleManager roleManager)
        {
            // wait for the database to be created
            await context.Database.EnsureCreatedAsync();

            var admin = new ApplicationUser("SooperDooperLifter")
            {
                Email = "thomasrea0113@gmail.com"
            };
            await userManager.CreateOrUpdateByNameAsync(admin, "Password123");

            var adminRole = new ApplicationRole("Admin");
            await roleManager.TryCreateByNameAsync(adminRole);

            await userManager.TryAddToRoleAsync(admin, adminRole.Name);

            var boards = await context.Set<LeaderboardModel>()
                .Where(b => b.Name == "Bench 1 Rep Max" || b.Name == "Deadlift 1 Rep Max")
                .ToListAsync();

            var ubs = context.Set<UserLeaderboard>();

            foreach (var board in boards)
                await ubs.FindOrAddAsync(UserLeaderboard.Create(admin, board), admin.Id, board.Id);

            await context.SaveChangesAsync();
            return builder;
        }
    }
}