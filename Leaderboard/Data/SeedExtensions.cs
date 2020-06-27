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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Leaderboard.Utilities;

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
        public static async Task<List<TEntity>> GetSeedDataFromFile<TEntity>(string environmentName)
        {
            var configuringType = typeof(TEntity);

            // seed data for this configuration, as well as all data for all configurations
            var seedFilePath = new string[]
            {
                Path.Combine(SeedDirectory, $"{configuringType.FullName}.data.json"),
                Path.Combine(SeedDirectory, $"{configuringType.FullName}.{environmentName}.data.json")
            };

            // for each of the possible seed file paths, load all the objects
            List<TEntity> objects = new List<TEntity>();
            foreach (var seedFile in seedFilePath)
                if (File.Exists(seedFile))
                {
                    using var reader = File.OpenRead(seedFile);
                    objects.AddRange(await JsonSerializer.DeserializeAsync<IEnumerable<TEntity>>(reader)
                    .ConfigureAwait(false));
                }

            return objects;
        }

        /// <summary>
        /// If either divisions/weightclasses are null, will add a single board with a null value for that property
        /// </summary>
        /// <param name="uomId"></param>
        /// <param name="divisions"></param>
        /// <param name="weightClasses"></param>
        /// <param name="boardNames"></param>
        /// <returns></returns>
        private static IEnumerable<LeaderboardModel> GenerateLeaderboards(
            string uomId,
            List<Division> divisions,
            List<WeightClass> weightClasses,
            params string[] boardNames)
        {
            divisions ??= new List<Division> { null };
            weightClasses ??= new List<WeightClass> { null };

            var allCombinations = from d in divisions
                                  from wc in weightClasses
                                  select (d, wc);

            var count = allCombinations.Count();

            LeaderboardModel generateBoard(
                string name,
                string divisionId,
                string weightClassId
            ) => new LeaderboardModel
            {
                Id = GuidUtilities.Create($"lb_{weightClassId}{divisionId}{name}").ToString(),
                Name = name,
                IsActive = true,
                WeightClassId = weightClassId,
                DivisionId = divisionId,
                UOMId = uomId
            };

            foreach ((var division, var weightClass) in allCombinations.Distinct())
                foreach (var boardName in boardNames)
                    yield return generateBoard(boardName, division?.Id, weightClass?.Id);
        }

        private static async IAsyncEnumerable<ApplicationRole> GetOrCreateRoles(this AppRoleManager manager, params string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                var role = new ApplicationRole(roleName)
                {
                    Id = GuidUtilities.Create($"role_{roleName}").ToString()
                };
                await manager.TryCreateByNameAsync(role).ConfigureAwait(false);
                yield return role;
            }
        }

        private static async IAsyncEnumerable<ApplicationUser> GetOrCreateUsers(this AppUserManager manager, GenderValue? gender, params string[] userNames)
        {
            foreach (var userName in userNames)
            {
                var user = new ApplicationUser(userName)
                {
                    Id = GuidUtilities.Create($"u_{userName}").ToString(),
                    Gender = gender,
                    IsActive = true
                };
                await manager.CreateOrUpdateByNameAsync(user, "Password123").ConfigureAwait(false);
                yield return user;
            }
        }

        private static async IAsyncEnumerable<UserLeaderboard> GenerateUserLeaderboards(
            IAsyncEnumerable<ValueTuple<ApplicationUser, List<LeaderboardModel>>> users)
        {
            await foreach ((var user, var boards) in users)
                foreach (var board in boards)
                    yield return new UserLeaderboard
                    {
                        Id = GuidUtilities.Create($"ub_{board.Id}{user.Id}").ToString(),
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
                    yield return new ScoreModel
                    {
                        Id = GuidUtilities.Create($"score_{board.UserId}{board.LeaderboardId}{i}").ToString(),
                        IsApproved = i % 2 == 0, // if i is even, then true. All odd indexes will be false,
                        UserId = board.UserId,
                        BoardId = board.LeaderboardId,
                        Value = Convert.ToDecimal(rand.NextDouble() * rand.Next(200, 1500))
                    };
                }
            }
        }

        private static IEnumerable<UserCategory> GenerateUserCategories(string categoryId, params ApplicationUser[] users)
            => users.Select(u => new UserCategory
            {
                Id = GuidUtilities.Create($"uc{u.Id}{categoryId}").ToString(),
                UserId = u.Id,
                CategoryId = categoryId
            });

        private static List<DivisionCategory> GenerateDivisionCategories(List<Division> divisions, string categoryId)
            => divisions.Select(d => new DivisionCategory
            {
                Id = GuidUtilities.Create($"dc_{d.Id}{categoryId}").ToString(),
                CategoryId = categoryId,
                DivisionId = d.Id
            }).ToList();

        /// <summary>
        /// Allows for identity user/role seeding using the provided managers.
        /// It's Important that you don't wrap any services
        /// in a using clause - the scope will likely be reused throughout the
        /// startup process. DI will handle the disposing for us
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task SeedDataAsync(
            this IServiceProvider services,
            string environmentName)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<AppUserManager>();
            var roleManager = services.GetRequiredService<AppRoleManager>();

            // wait for the database to be created
            await context.Database.EnsureCreatedAsync().ConfigureAwait(false);

            // see below for an example of how to use the different bulk insert overloads
            var weightClasses = await GetSeedDataFromFile<WeightClass>(environmentName).ConfigureAwait(false);
            await context.BulkInsertOrUpdateAsync(t => new { t.Id }, weightClasses.ToArray()).ConfigureAwait(false);

            var divisions = await GetSeedDataFromFile<Division>(environmentName).ConfigureAwait(false);
            await context.BulkInsertOrUpdateAsync((elist, e) => elist.Any(e2 => e2.Id == e.Id), divisions.ToArray()).ConfigureAwait(false);

            await context.SaveChangesAsync().ConfigureAwait(false);

            // categoryId from Category.cs call to HasData()
            var divisionCategories = GenerateDivisionCategories(
                divisions.Where(d => d.Id != "61a38bd5-49f1-4717-bd65-ab795da6fe26").ToList(),
                "642313a2-1f0c-4329-a676-7a9cdac045bd");
            await context.BulkInsertOrUpdateAsync(divisionCategories.ToArray()).ConfigureAwait(false);

            // adding the one running division to the running category
            var runningDivisions = divisions.Where(d => d.Id == "61a38bd5-49f1-4717-bd65-ab795da6fe26" ||
            d.Id == "47a6f903-e450-45a3-8793-15856c9bc88f").ToList();
            divisionCategories = GenerateDivisionCategories(runningDivisions, "6772a358-e5b7-49dd-a49b-9d855ed46c5e");
            await context.BulkInsertOrUpdateAsync(divisionCategories.ToArray()).ConfigureAwait(false);

            var divisionWeightClasses = await GetSeedDataFromFile<DivisionWeightClass>(environmentName).ConfigureAwait(false);
            await context.BulkInsertOrUpdateAsync(divisionWeightClasses.ToArray()).ConfigureAwait(false);

            // adding bench/squat/deadlift to all divisions/weightclasses
            var boards = GenerateLeaderboards("e362dd90-d6fe-459b-ba26-09db002bfff6", divisions, weightClasses, "Bench", "Squat", "Deadlift");
            await context.BulkInsertOrUpdateAsync(boards.ToArray()).ConfigureAwait(false);

            await context.SaveChangesAsync().ConfigureAwait(false);

            var roles = await roleManager.GetOrCreateRoles("admin").ToListAsync().ConfigureAwait(false);
            var users = await userManager.GetOrCreateUsers(null, "Admin").ToListAsync().ConfigureAwait(false);
            await userManager.TryAddToRoleAsync(users.First(), "Admin").ConfigureAwait(false);

            await context.SaveChangesAsync().ConfigureAwait(false);

            // if not in productions, we want some dummy users and scores
            if (environmentName != "Production")
            {

                // adding some null data for division/weight class. Important for testing.
                // TODO determine real age/weight groups for sprinting and add more races
                var sprintBoards = GenerateLeaderboards("12c7c15a-db13-4912-a7c8-fc86db54849b", runningDivisions, null, "100-Metre Dash", "40-Yard Dash");
                await context.BulkInsertOrUpdateAsync(sprintBoards.ToArray()).ConfigureAwait(false);

                await context.SaveChangesAsync().ConfigureAwait(false);

                users = await userManager.GetOrCreateUsers(GenderValue.Male, "LifterDuder", "LiftLife", "Lifter22")
                    .ToListAsync().ConfigureAwait(false);

                var ld = users.First();
                ld.Weight = 75;
                await userManager.UpdateAsync(ld).ConfigureAwait(false);

                var llUser = users.Single(u => u.UserName == "LiftLife");
                var email = $"thomasrea0113@gmail.com";
                llUser.Email = email;
                llUser.NormalizedEmail = email.ToUpper();
                await userManager.UpdateAsync(llUser).ConfigureAwait(false);

                // setting some birthdates
                users[0].BirthDate = DateTime.Parse("05/11/1993");
                users[1].BirthDate = DateTime.Parse("01/12/2011");
                users[2].BirthDate = DateTime.Parse("05/11/2099");

                var userCats = GenerateUserCategories("642313a2-1f0c-4329-a676-7a9cdac045bd", users.SkipLast(1).ToArray());
                userCats = userCats.Concat(GenerateUserCategories("6772a358-e5b7-49dd-a49b-9d855ed46c5e", users.TakeLast(1).ToArray()));
                await context.BulkInsertOrUpdateAsync(userCats.ToArray()).ConfigureAwait(false);

                boards = GenerateLeaderboards("e362dd90-d6fe-459b-ba26-09db002bfff6", divisions, null, "Null board weight 1", "Null weight board 2");
                await context.BulkInsertOrUpdateAsync(boards.ToArray()).ConfigureAwait(false);

                await context.SaveChangesAsync().ConfigureAwait(false);

                // only add the user to half of there recommendations
                var recommendations = users.ToAsyncEnumerable()
                    .SelectAwait(async u =>
                    {
                        var recommendations = await userManager.GetRecommendedBoardsQuery(u)
                            .ToListAsync().ConfigureAwait(false);
                        if (recommendations.Any())
                            return (u, recommendations.Skip(recommendations.Count / 2).ToList());
                        return default;
                    })
                    .Where(ub => ub != default);

                var userBoards = await GenerateUserLeaderboards(recommendations)
                    .ToArrayAsync().ConfigureAwait(false);
                await context.BulkInsertOrUpdateAsync(e => new { e.UserId, e.LeaderboardId }, userBoards)
                    .ConfigureAwait(false);

                var scores = GenerateScores(userBoards.ToList());
                await context.BulkInsertOrUpdateAsync(scores.ToArray()).ConfigureAwait(false);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}