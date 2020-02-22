using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Data;
using Leaderboard.Models.Relationships;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Areas.Identity.Managers
{
    public class AppUserStore : UserStore<
        ApplicationUser,
        ApplicationRole,
        ApplicationDbContext,
        string,
        ApplicationUserClaim,
        ApplicationUserRole,
        ApplicationUserLogin,
        ApplicationUserToken,
        ApplicationRoleClaim
    >
    {
        public AppUserStore(ApplicationDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        /// <summary>
        /// Joins all the divisions on the users categories. This will not filter out
        /// divisions that the user is already a part of
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<List<LeaderboardModel>> GetRecommendedBoardsAsync(ApplicationUser user)
        {
            // LINQ to Queries is quite powerful! The key to remember is to NEVER use a navigation
            // property in your query. Always use the DbSet on the context, and then use joins
            var recommendations = from b in Context.Leaderboards.AsQueryable()
                from wc in Context.WeightClasses
                from d in Context.Divisions

                // getting only divisions that overlap with the users categories
                join uc in Context.UserCategories on user.Id equals uc.UserId
                join dc in Context.DivisionCategories on d.Id equals dc.DivisionId
                where dc.CategoryId == uc.CategoryId &&
                    // // if the gender/age on the division is null, then this user automatically qualifies
                    (
                        b.DivisionId == null ||
                        (
                            (d.Gender == null || user.Gender == d.Gender) &&
                            (d.AgeLowerBound == null || d.AgeLowerBound < user.Age) &&
                            (d.AgeUpperBound == null || user.Age <= d.AgeUpperBound)
                        )
                    ) &&

                    // if the board has no weight class, the user automatically qualifies
                    (
                        b.WeightClassId == null ||
                        (
                            wc.WeightLowerBound < user.Weight &&
                            user.Weight <= wc.WeightUpperBound
                        )
                    )

                select b;
            return await recommendations.ToListAsync();
        }

        public async Task<IdentityResult> CreateOrFindByIdAsync(ApplicationUser user)
        {
            var newEntry = Context.Entry(user);

            if (newEntry.State != EntityState.Detached)
                throw new InvalidOperationException($"This method should only be called on an entity that is in state '{EntityState.Detached}'");

            var existing = await FindByIdAsync(user.Id);
            if(existing != null)
            {
                // if the user exists, return it and overwrite passed values. 
                var dbEntry = Context.Entry(existing);
                newEntry.CurrentValues.SetValues(dbEntry.CurrentValues);

                // now that we've set the values of the new entry, we want to also detach
                // the entry we just found above, and mark the passed in entry as unmodified
                // TODO implement a CreateOrUpdateByIdAsync method, that first compares the two PropertyValues collections
                dbEntry.State = EntityState.Detached;
                newEntry.State = EntityState.Unchanged;

                // because we overwrote the passed in user with the dbValues,
                // no identity action was performed.
                return null;
            }
            else
                return await CreateAsync(user);
        }
    }
}
