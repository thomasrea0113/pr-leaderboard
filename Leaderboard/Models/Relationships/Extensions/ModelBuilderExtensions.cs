using System;
using System.Collections.Generic;
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
        private static Regex _modelRegex = new Regex("Model$");

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
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

            return IsAssignableToGenericType(baseType, genericType);
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

        /// <summary>
        /// Dynamically adds the tables needed for many-to-many relationships
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="relationshipType"></param>
        public static void AddRelationship(this ModelBuilder modelBuilder, Type relationshipType)
        {
            var abstractRelationshipType = typeof(AbstractRelationship<,>);
            if (!IsAssignableToGenericType(relationshipType, abstractRelationshipType))
                throw new ArgumentException($"type {relationshipType.FullName} doesn't inherit from {abstractRelationshipType}");


            foreach ((var mt, var rcp, var fkp) in GetRelationshipProperties(relationshipType))
                modelBuilder.Entity(relationshipType)
                    .HasOne(mt)
                    .WithMany(rcp)
                    .HasForeignKey(fkp);

            // modelBuilder.Entity<UserLeaderboard>()
            //     .HasOne(pt => pt.Leaderboard)
            //     .WithMany(p => p.UserLeaderboards)
            //     .HasForeignKey(pt => pt.UserId);
        }

        /// <summary>
        /// Add all the <see cref="AbstractRelationship{TModel1,TModel2}" /> defined in the
        /// given assembly. If no assembly is provided, uses the executing assembly.
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="assembly"></param>
        public static void AddAllRelationships(this ModelBuilder modelBuilder, Assembly assembly = default)
        {
            if (assembly == default)
                assembly = Assembly.GetExecutingAssembly();

            var abstractRelationshipType = typeof(AbstractRelationship<,>);
            foreach(var relationshipType in assembly.GetTypes()
                .Where(t => !t.IsAbstract && t.BaseType == abstractRelationshipType))
                modelBuilder.AddRelationship(relationshipType);
        }

        public static void AddCompositeKey(this ModelBuilder modelBuilder, Type modelType)
        {
            var keys = modelType.GetProperties()
                .Select(p => new {Prop = p, Attr = p.GetCustomAttribute<CompositeKeyAttribute>(false)})
                .Where(ca => ca.Attr != null)
                .Select(ca => ca.Prop.Name);

            if (keys.Any())
                modelBuilder.Entity(modelType).HasKey(keys.ToArray());
        }

        public static void AddCompositeKeys(this ModelBuilder modelBuilder, Assembly assembly = default)
        {
            if (assembly == default)
                assembly = Assembly.GetExecutingAssembly();

            // all the properties have to be inspected, so it's quicker to enumerate all the defined types
            foreach(var type in assembly.GetTypes())
                modelBuilder.AddCompositeKey(type);
        }
    }
}