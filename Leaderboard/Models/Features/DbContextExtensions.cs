using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static Leaderboard.Utilities.SlugUtilities;

namespace Leaderboard.Models.Features
{
    public static class DbContextExtensions
    {
        public static IQueryable Query(this DbContext context, Type entityType)
        {
            var SetMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set));
            return (IQueryable)SetMethod.MakeGenericMethod(entityType).Invoke(context, null);
        }

        // same as the above, except it assumes the return type. Useful for models that
        // share a common interface
        public static IQueryable<T> Query<T>(this DbContext context, Type entityType)
        {
            var queryable = Query(context, entityType);
            return (IQueryable<T>)queryable;
        }

        public static void ForEachDbSetEntity(this DbContext ctx, Action<Type> action, Func<Type, bool> condition = default)
        {
            var dbSetType = typeof(DbSet<>);
            var entityTypes = ctx.GetType().GetProperties()
                .Select(p => p.PropertyType)
                .Where(t => t.IsGenericType && dbSetType.IsAssignableFrom(t.GetGenericTypeDefinition()))
                .Select(t => t.GenericTypeArguments[0]);

            foreach (var entityType in entityTypes)
                if (condition == default || condition(entityType))
                    action(entityType);
        }

        /// <summary>
        /// Gets all DbSets of an entity type that implements IDbEntity<> and calls the configure method
        /// on each entity
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="builder"></param>
        public static void ConfigureEntities(this DbContext ctx, ModelBuilder builder)
            => ForEachDbSetEntity(ctx, configuringType =>
        {
            // IDbEntity<> has a constraint stating that classes that implement it must
            // have a public parameterless constructor
            object entityInstance = Activator.CreateInstance(configuringType);

            // the method on the builder that creates the EntityTypeBuilder instance
            var entityMethod = builder.GetType()
                .GetMethod(nameof(builder.Entity), 1, Array.Empty<Type>())
                .MakeGenericMethod(configuringType);

            // the implemented IDbEntry OnModelConfiguring method. Using object as the type paraemeter
            // because the method name is indepenedent of the type parameter
            var modelCreatingMethod = configuringType.GetMethod(nameof(IDbEntity<object>.OnModelCreating));

            // invoking the OnModelCreating method, passing the EntityTypeBuilder parameter
            modelCreatingMethod.Invoke(entityInstance, new object[] { entityMethod.Invoke(builder, null) });

        },
        // The entity type can only configure itself. So if the generic parameter does not match
        // the type that implements the interface, we won't process it
        t => typeof(IDbEntity<>).MakeGenericType(t).IsAssignableFrom(t));

        public static async Task ProcessPreSaveFeaturesAsync(this IEnumerable<EntityEntry> entries)
        {

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    if (entry.Entity is IOnDbPreSaveAsync onSave)
                        await onSave.OnPreSaveAsync(entry.Context, entry.CurrentValues).ConfigureAwait(false);
                    if (entry.Entity is ISlugged slugged)
                        slugged.Slug = Slugify(slugged.Name);
                }

                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is IOnDbPreCreateAsync onCreate)
                        await onCreate.OnPreCreateAsync(entry.Context, entry.CurrentValues).ConfigureAwait(false);
                }
                else if (entry.State == EntityState.Deleted)
                {

                    // if this model has an active feature, then we prevent the delete
                    // and set active to false
                    if (entry.Entity is IDbActive active)
                    {
                        active.IsActive = false;
                        entry.State = EntityState.Modified;
                    }
                    else
                    {
                        // if not active, continue processing actions
                        if (entry.Entity is IOnDbPreDeleteAsync onDelete)
                            await onDelete.OnDeleteAsync(entry.Context).ConfigureAwait(false);
                    }
                }
            }

            var deleted = entries.Where(t => t.State == EntityState.Deleted);
            foreach (var entry in deleted)
            {
                var entity = entry.Entity;

                if (entity is IOnDbPreDeleteAsync onDelete)
                    await onDelete.OnDeleteAsync(entry.Context).ConfigureAwait(false);

                // if this model has an active feature, then we prevent the delete
                // and set active to false
                // TODO notify user somewhere of prevents deletion?
                if (entity is IDbActive active)
                {
                    active.IsActive = false;
                    entry.State = EntityState.Modified;
                }
            }
        }

        public static Task ProcessPostSaveFeaturesAsync(this IEnumerable<EntityEntry> _)
        {
            // stubbed 
            return Task.CompletedTask;
        }
    }
}