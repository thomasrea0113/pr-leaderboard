using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Data
{
    public static class BulkExtensions
    {
        /// <summary>
        /// The simplest way to bulk insert or update. Will assume that the provided Entity type
        /// has a property called 'Id', which is the PK for the entity in the database
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entities"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task BulkInsertOrUpdateAsync<T>(this DbContext context, params T[] entities)
            where T : class => await context.BulkInsertOrUpdateAsync(t => new { Id = "" }, entities).ConfigureAwait(false);

        /// <summary>
        /// The second easiest way to bulk insert/update. Useful for entities with a non-standard primary key.
        /// the keys Func should return an anonymous object that contains the key properties on that object.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="keys"></param>
        /// <param name="entities"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task BulkInsertOrUpdateAsync<T>(this DbContext context, Expression<Func<T, object>> keys, params T[] entities)
            where T : class
        {
            if (!entities.Any())
                throw new ArgumentNullException("at least one entity must be provided");

            var keyObject = keys.Compile()(entities.First());
            var keyPropNames = keyObject.GetType().GetProperties().Select(p => p.Name).ToList();
            var keyProps = typeof(T).GetProperties().Where(p => keyPropNames.Contains(p.Name)).ToList();

            if (keyPropNames.Count != keyProps.Count)
                throw new ArgumentException($"model type {typeof(T).FullName} did not contain all keys: {string.Join(", ", keyPropNames)}");

            bool equality(List<T> elist, T e) => elist.Any(e2 =>
            {
                foreach (var prop in keyProps)
                    if (!prop.GetValue(e2).Equals(prop.GetValue(e)))
                        return false;
                return true;
            });

            await context.BulkInsertOrUpdateAsync(equality, entities).ConfigureAwait(false);
        }


        /// <summary>
        /// If equality is more complex than just directly comparing a couple of fields, then you can provide your own
        /// eqality delegate
        /// </summary>
        /// <param name="context"></param>
        /// <param name="equality"></param>
        /// <param name="entities"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task BulkInsertOrUpdateAsync<T>(this DbContext context, Func<List<T>, T, bool> equality, params T[] entities)
            where T : class
        {
            var set = context.Set<T>();

            var existing = await set.AsQueryable().Where(e => entities.Contains(e)).ToListAsync().ConfigureAwait(false);

            var updatedEntities = entities.Where(e => equality(existing, e)).ToList();
            var newEntitities = entities.Where(e => !equality(existing, e)).ToList();

            // the above call to ToListAsync will attach any existing entities
            // to the context. Because we want to update them with the passed in
            // entries, we need to detach what we just attached.
            foreach (var tracked in existing)
                context.Entry(tracked).State = EntityState.Detached;

            var anyNew = newEntitities.Count > 0;
            var anyUpdated = updatedEntities.Count > 0;

            if (anyNew)
                await set.AddRangeAsync(newEntitities).ConfigureAwait(false);
            if (anyUpdated)
                set.UpdateRange(updatedEntities);
        }
    }
}