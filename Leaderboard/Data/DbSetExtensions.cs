using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Leaderboard.Data.Extensions
{
    public static class DbSetExtensions
    {
        public static async Task<ValueTuple<bool, T>> FindOrAddAsync<T>(this DbSet<T> set, T entity, params object[] keys)
            where T : class
        {
            var found = await set.FindAsync(keys);
            if (found == default)
                return (true, (await set.AddAsync(entity)).Entity);
            return (false, found);
        }
    }
}