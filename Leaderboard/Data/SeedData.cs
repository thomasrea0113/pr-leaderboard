using System;
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
        public static string[] UserIds = new string[]
        {
            "admin-user-id",
            "test-user-1-id",
            "test-user-2-id",
            "test-user-3-id",
            "test-user-4-id",
        };

        /// <summary>
        /// Allows for identity user/role seeding using the provided managers.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<IApplicationBuilder> SeedDevelopmentDataAsync(this IApplicationBuilder builder, DbContext context,
        AppUserManager userManager, AppRoleManager roleManager)
        {
            // wait for the database to be created
            await context.Database.EnsureCreatedAsync();

            var users = UserIds.Select(id => new ApplicationUser($"{id}-username")
            {
                Id = id,
                Email = $"{id}@gmail.com"
            }).ToArray();

            foreach (var user in users)
                await userManager.CreateOrUpdateByNameAsync(user, "Password123");

            var adminRole = new ApplicationRole("Admin");
            await roleManager.TryCreateByNameAsync(adminRole);

            await userManager.TryAddToRoleAsync(users[0], adminRole.Name);

            var boards = await context.Set<LeaderboardModel>().ToListAsync();

            var ubs = context.Set<UserLeaderboard>();

            // add all users to all boards
            foreach (var user in users)
                foreach (var board in boards)
                    await ubs.FindOrAddAsync(UserLeaderboard.Create(user, board), user.Id, board.Id);
                    
            var  SeedIds = Enumerable.Range(0, 150)
                .Select(i => $"score-{i}-id")
                .ToArray();
            
            var rand = new Random();
            var scores = SeedIds.Select((id, index) => new ScoreModel
            {
                Id = id,
                BoardId = LeaderboardModel.SeedIds[index % 3],
                UserId = SeedExtensions.UserIds[index % 5],
                Value = Convert.ToDecimal(rand.NextDouble()) * 100
            });

            var scoreSet = context.Set<ScoreModel>();
            foreach (var score in scores)
                await scoreSet.FindOrAddAsync(score, score.Id);

            await context.SaveChangesAsync();
            return builder;
        }
    }
}