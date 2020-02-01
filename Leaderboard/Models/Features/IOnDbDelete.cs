using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Leaderboard.Models.Features
{
    public interface IOnDbDelete
    {
        void OnDelete(DbContext ctx);
    }
}