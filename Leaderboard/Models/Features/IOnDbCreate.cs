using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Leaderboard.Models.Features
{
    public interface IOnDbCreate
    {
        void OnCreate(DbContext ctx, PropertyValues values);
    }
}