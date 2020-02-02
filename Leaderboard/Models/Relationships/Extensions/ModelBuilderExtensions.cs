using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Models.Relationships.Extensions
{
    public static class ModelBuilderExtensions
    {
        #region helpers

        private static Regex _modelRegex = new Regex("Model$");

        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return baseType.IsAssignableToGenericType(genericType);
        }

        private static ValueTuple<Type, string, string> GetRelationshipProperties(
            Type relationshipType, Type mt1, Type mt2)
        {
            var relationshipCollectionType = typeof(ICollection<>).MakeGenericType(relationshipType);
            var relationshipCollectionPropertyName = mt1.GetProperties()
                .SingleOrDefault(p => relationshipCollectionType.IsAssignableFrom(p.PropertyType))?.Name;

            if (relationshipCollectionPropertyName == default)
                throw new ArgumentException($"model of type {mt1.FullName} does not have a property of type {relationshipType}");

            var mt2IdPropertyName = _modelRegex.Replace(mt2.Name, "Id");

            if (!mt2IdPropertyName.EndsWith("Id"))
                throw new ArgumentException($"The name of type {mt2.FullName} does not end in Model");

            var mt2IdName = relationshipType.GetProperties().SingleOrDefault(p => p.Name == mt2IdPropertyName)?.Name;

            if (mt2IdName == default)
                throw new ArgumentException($"type {relationshipType.FullName} does not have a public property named {mt2IdPropertyName}");

            return (mt1, relationshipCollectionPropertyName, mt2IdName);
        }

        private static IEnumerable<ValueTuple<Type, string, string>> GetRelationshipProperties(Type relationshipType)
        {
            // each relationship class must directly inherit the AbstractRelationship type
            var typeParameters = relationshipType.BaseType.GenericTypeArguments;
            var mt1 = typeParameters[0];
            var mt2 = typeParameters[1];

            yield return GetRelationshipProperties(relationshipType, mt1, mt2);
            yield return GetRelationshipProperties(relationshipType, mt2, mt1);
        }

        public static void ForEachEntityType(this DbContext context, Action<Type> action)
        {
            var dbSetType = typeof(DbSet<>);
            foreach (var type in context.GetType()
                .GetProperties()
                .Select(p => p.PropertyType)
                .Where(t => t.IsAssignableToGenericType(dbSetType))
                .Select(t => t.GenericTypeArguments[0]))
                action(type);
        }

        #endregion

        /// <summary>
        /// Add all the <see cref="AbstractRelationship{TModel1,TModel2}" /> defined in the
        /// given assembly. If no assembly is provided, uses the executing assembly.
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="assembly"></param>
        public static void AddAllRelationships(this ModelBuilder modelBuilder, DbContext context)
            => context.ForEachEntityType(type =>
            {
                var abstractRelationshipType = typeof(AbstractRelationship<,>);

                if (type.IsAssignableToGenericType(abstractRelationshipType))
                {
                    var entity = modelBuilder.Entity(type);
                    foreach ((var mt, var rcp, var fkp) in GetRelationshipProperties(type))
                        entity.HasOne(mt)
                        .WithMany(rcp)
                        .HasForeignKey(fkp);
                }
            });

        public static void AddCompositeKeys(this ModelBuilder modelBuilder, DbContext context)
            => context.ForEachEntityType(type =>
            {
                var keys = type.GetProperties()
                    .Where(p => p.GetCustomAttribute<CompositeKeyAttribute>(false) != default)
                    .Select(p => p.Name);

                if (keys.Any())
                    modelBuilder.Entity(type).HasKey(keys.ToArray());
            });

        /// <summary>
        /// Add all the default values from data annotations for all the models in the given
        /// context
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="context"></param>
        public static void AddDefaultValues(this ModelBuilder modelBuilder, DbContext context)
            => context.ForEachEntityType(type =>
            {
                var defaults = type.GetProperties()
                    .Select(p => new
                    {
                        Prop = p.Name,
                        Val = p.GetCustomAttribute<DefaultValueAttribute>(false)?.Value
                    })
                    .Where(dv => dv.Val != null);

                if (defaults.Any())
                {
                    var entity = modelBuilder.Entity(type);
                    foreach (var dv in defaults)
                        entity.Property(dv.Prop).HasDefaultValue(dv.Val);
                }
            });
    }
}