using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Data.Extensions
{
    public static class DbSetExtensions
    {
        public static async IAsyncEnumerable<T> FindOrAdd<T>(this DbSet<T> set, params T[] entities)
            where T : class
        {
            foreach (var tag in entities)
            {
                var found = await set.FindAsync(tag);
                if (found == default)
                    found = (await set.AddAsync(tag)).Entity;
                yield return found;
            }
        }
    }
}