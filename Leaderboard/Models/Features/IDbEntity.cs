using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leaderboard.Models.Features
{
    public interface IDbEntity<TEntity>
        where TEntity : class, new()
    {
        void OnModelCreating(EntityTypeBuilder<TEntity> builder);
    }
}