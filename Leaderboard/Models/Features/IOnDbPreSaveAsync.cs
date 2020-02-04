using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Leaderboard.Models.Features
{
    public interface IOnDbPreSaveAsync
    {
        Task OnPreSaveAsync(DbContext ctx, PropertyValues values);
    }
}