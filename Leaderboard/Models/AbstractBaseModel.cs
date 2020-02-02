using System.Threading.Tasks;
using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Leaderboard.Models
{
    public abstract class AbstractBaseModel : IOnDbPreCreateAsync, IOnDbSave, IOnDbDelete
    {
        public virtual Task OnPreCreateAsync(DbContext ctx, PropertyValues values)
        {
            return Task.CompletedTask;
        }

        public virtual void OnDelete(DbContext ctx)
        {
        }

        public virtual void OnSave(DbContext ctx, PropertyValues values)
        {
        }
    }
}