using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Models.Features
{
    public static class DbContextExtensions
    {
        private static void ForEachDbSetEntity(this DbContext ctx, Action<Type> action, Func<Type, bool> condition = default)
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
    }
}