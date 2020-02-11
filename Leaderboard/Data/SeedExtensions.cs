using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Managers;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Data.BulkExtensions;
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

            await context.SaveChangesAsync();
            return builder;
        }
    }
}