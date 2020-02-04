using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Leaderboard.Models.Features
{
    public interface IOnDbPreDeleteAsync
    {
        Task OnDeleteAsync(DbContext ctx);
    }
}