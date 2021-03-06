using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Models.Features
{
    public interface IOnDbPreDeleteAsync
    {
        Task OnDeleteAsync(DbContext ctx);
    }
}