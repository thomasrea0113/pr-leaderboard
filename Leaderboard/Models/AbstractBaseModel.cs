using Leaderboard.Models.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Leaderboard.Models
{
    public abstract class AbstractBaseModel : IOnDbCreate, IOnDbSave, IOnDbDelete
    {
        public virtual void OnCreate(DbContext ctx, PropertyValues values)
        {
        }

        public virtual void OnDelete(DbContext ctx)
        {
        }

        public virtual void OnSave(DbContext ctx, PropertyValues values)
        {
        }
    }
}