using Arch.EntityFrameworkCore.UnitOfWork;
using Leaderboard.Areas.Profiles.Models;
using Leaderboard.Data;

namespace Leaderboard.Areas.Profiles.data
{
    public class ProfileRepository : Repository<UserProfileModel>
    {
        public ProfileRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}