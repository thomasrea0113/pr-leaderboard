using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Models.Features
{
    /// <summary>
    /// Will prevent the model from being deleted, and instead simply be set as inactive
    /// </summary>
    public interface IDbActive
    {
        bool? IsActive { get; set; }
    }

    public static class DbSetExtensions
    {
        public static async ValueTask<TModel> FindActiveAsync<TModel>(this DbSet<TModel> set, params object[] keyValues)
            where TModel : class, IDbActive
        {
            var found = await set.FindAsync(keyValues);
            if (!found.IsActive.HasValue || !found.IsActive.Value)
                return default;
            return found;
        }

        public static IQueryable<TModel> WhereActive<TModel>(this DbSet<TModel> set)
            where TModel : class, IDbActive
            => set.AsQueryable().Where(m => m.IsActive ?? false);
    }
}