using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Leaderboard.Models.Features
{
    public static class DbContextExtensions
    {
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
            
            var added = entries.Where(t => t.State == EntityState.Added);
            foreach (var entry in added)
            {
                var entity = entry.Entity;

                if (entry.Entity is IOnDbPreCreateAsync onCreate)
                    await onCreate.OnPreCreateAsync(entry.Context, entry.CurrentValues);
            }

            var modified = entries.Where(t => t.State == EntityState.Modified);
            foreach (var entry in modified)
            {
                var entity = entry.Entity;

                if (entity is IOnDbPreSaveAsync onSave)
                    await onSave.OnPreSaveAsync(entry.Context, entry.CurrentValues);
            }

            var deleted = entries.Where(t => t.State == EntityState.Deleted);
            foreach (var entry in deleted)
            {
                var entity = entry.Entity;

                if (entity is IOnDbPreDeleteAsync onDelete)
                    await onDelete.OnDeleteAsync(entry.Context);

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