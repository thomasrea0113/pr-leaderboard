using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Models.Features
{
    public interface IDbSeed<TEntity>
        where TEntity : class, new()
    {
        /// <summary>
        /// Should be implemented by the TEntity model class. Any object implementing
        /// this interface must also have a DbSet&lt;TEntity&gt; on the DbContext. If
        /// the entity must provide production data, it can use the HasData function
        /// regardless of the requestSeed parameter
        /// </summary>P
        /// <param name="builder"></param>
        /// <param name="requestSeed">Indicates if we are seeding a development database with test data</param>
        void SeedData(EntityTypeBuilder<TEntity> builder);
    }
}