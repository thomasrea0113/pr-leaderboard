using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Data.BulkExtensions;
using Leaderboard.Models.Relationships;
using Leaderboard.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Leaderboard.Data.SeedExtensions
{
    public static class SeedExtensions
    {

        // the local data where default production data is stored
        public static string SeedDirectory { get; }
            = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data", "Seed");

        /// <summary>
        /// Loads all the data for the given type.
        /// </summary>
        public static async Task<List<TEntity>> GetSeedDataFromFile<TEntity>(this IApplicationBuilder builder, string environmentName, params string[] keyNames)
        {
            var configuringType = typeof(TEntity);

            // seed data for this configuration, as well as all data for all configurations
            var seedFilePath = new string []
            {
                Path.Combine(SeedDirectory, $"{configuringType.FullName}.data.json"),
                Path.Combine(SeedDirectory, $"{configuringType.FullName}.{environmentName}.data.json")
            };

            // for each of the possible seed file paths, load all the objects
            List<TEntity> objects = new List<TEntity>();
            foreach (var seedFile in seedFilePath)
                if (File.Exists(seedFile))
                    objects.AddRange(await JsonSerializer.DeserializeAsync<IEnumerable<TEntity>>(File.OpenRead(seedFile)));

            return objects;
        }

        private static IEnumerable<LeaderboardModel> GenerateLeaderboards(List<Division> divisions, List<WeightClass> weightClasses)
        {
            foreach (var division in divisions)
                foreach (var weightClass in weightClasses)
                {
                    Func<string, LeaderboardModel> generateBoard = name
                        => new LeaderboardModel {
                            Id = GuidUtility.Create(GuidUtility.UrlNamespace, $"lb_{weightClass.Id}{division.Id}{name}").ToString(),
                            Name = name,
                            IsActive = true,
                            WeightClassId = weightClass.Id,
                            DivisionId = division.Id,
                            UOMId = "e362dd90-d6fe-459b-ba26-09db002bfff6"
                        };

                    yield return generateBoard("Bench");
                    yield return generateBoard("Squat");
                    yield return generateBoard("Deadlift");
                }
        }

        private static async IAsyncEnumerable<ApplicationRole> GetOrCreateRoles(this AppRoleManager manager, params string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                var role = new ApplicationRole(roleName)
                {
                    Id = GuidUtility.Create(GuidUtility.UrlNamespace, $"role_{roleName}").ToString()
                };
                await manager.TryCreateByNameAsync(role);
                yield return role;
            }
        }

        private static async IAsyncEnumerable<ApplicationUser> GetOrCreateUsers(this AppUserManager manager, params string[] userNames)
        {
            foreach (var userName in userNames)
            {
                var user = new ApplicationUser(userName)
                {
                    Id = GuidUtility.Create(GuidUtility.UrlNamespace, $"u_{userName}").ToString(),
                    IsActive = true
                };
                await manager.CreateOrUpdateByNameAsync(user, "Password123");
                yield return user;
            }
        }

        private static IEnumerable<UserLeaderboard> GenerateUserLeaderboards(
            List<LeaderboardModel> boards,
            List<ApplicationUser> users)
        {
            foreach (var board in boards)
                foreach (var user in users)
                    yield return new UserLeaderboard
                    {
                        Id = GuidUtility.Create(GuidUtility.UrlNamespace, $"ub_{board.Id}{user.Id}").ToString(),
                        UserId = user.Id,
                        LeaderboardId = board.Id
                    };
        }

        private static IEnumerable<ScoreModel> GenerateScores(List<UserLeaderboard> userBoards)
        {
            // sudo random number generation. Always seed with 1, so the calls to Next are predictable
            var rand = new Random(1);

            foreach (var board in userBoards)
            {
                for (var i = 0; i < 10; i++)
                {
                    yield return new ScoreModel {
                        Id = GuidUtility.Create(GuidUtility.UrlNamespace, $"score_{board.UserId}{board.LeaderboardId}{i}").ToString(),
                        IsApproved = i % 2 == 0, // if i is even, then true. All odd indexes will be false,
                        UserId = board.UserId,
                        BoardId = board.LeaderboardId,
                        Value = Convert.ToDecimal(rand.NextDouble() * rand.Next(200, 1500))
                    };
                }
            }
        }

        /// <summary>
        /// Allows for identity user/role seeding using the provided managers.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<IApplicationBuilder> SeedDataAsync(
            this IApplicationBuilder builder,
            IServiceProvider services,
            string environmentName)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<AppUserManager>();
            var roleManager = services.GetRequiredService<AppRoleManager>();

            // wait for the database to be created
            await context.Database.EnsureCreatedAsync();

            // see below for an example of how to use the different bulk insert overloads
            var weightClasses = await builder.GetSeedDataFromFile<WeightClass>(environmentName);
            await context.BulkInsertOrUpdateAsync(t => new { t.Id }, weightClasses.ToArray());

            var divisions = await builder.GetSeedDataFromFile<Division>(environmentName);
            await context.BulkInsertOrUpdateAsync((elist, e) => elist.Any(e2 => e2.Id == e.Id), divisions.ToArray());
            
            await context.SaveChangesAsync();

            var divisionWeightClasses = await builder.GetSeedDataFromFile<DivisionWeightClass>(environmentName);
            await context.BulkInsertOrUpdateAsync(divisionWeightClasses.ToArray());

            var boards = GenerateLeaderboards(divisions, weightClasses);
            await context.BulkInsertOrUpdateAsync(boards.ToArray());

            await context.SaveChangesAsync();

            var roles = await roleManager.GetOrCreateRoles("admin").ToListAsync();
            var users = await userManager.GetOrCreateUsers("Admin").ToListAsync();
            await userManager.TryAddToRoleAsync(users.First(), "Admin");

            // if not in productions, we want some dummy users and scores
            if (environmentName != "production")
            {
                users = await userManager.GetOrCreateUsers("LifterDuder", "LiftLife").ToListAsync();

                var userBoards = GenerateUserLeaderboards(boards.ToList(), users);
                await context.BulkInsertOrUpdateAsync(e => new { e.UserId, e.LeaderboardId }, userBoards.ToArray());

                var scores = GenerateScores(userBoards.ToList());
                await context.BulkInsertOrUpdateAsync(scores.ToArray());
            }

            await context.SaveChangesAsync();
            return builder;
        }
    }
}