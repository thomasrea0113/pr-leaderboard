using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Leaderboard.Models.Features
{
    public interface IOnDbSave
    {
        void OnSave(DbContext ctx, PropertyValues values);
    }
}