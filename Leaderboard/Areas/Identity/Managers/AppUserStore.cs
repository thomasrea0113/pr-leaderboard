using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Data;
using Leaderboard.Extensions;
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
        public IQueryable<LeaderboardModel> GetRecommendedBoardsQuery(ApplicationUser user)
        {
            // LINQ to Queries is quite powerful! The key to remember is to NEVER use a navigation
            // property in your query. Always use the DbSet on the context, and then use joins
            var recommendations = from b in Context.Leaderboards.AsQueryable()
                join d in Context.Divisions on b.DivisionId equals d.Id

                // left join, because a board may not have a weight class
                join wc in Context.WeightClasses on b.WeightClassId equals wc.Id into gr
                from wc in gr.DefaultIfEmpty()

                // board is active
                where b.IsActive == true

                // there are overlapping categories
                where Context.UserCategories.AsQueryable().Where(uc => uc.UserId == user.Id)
                    .Join(Context.DivisionCategories.AsQueryable().Where(dc => dc.DivisionId == d.Id),
                        uc => uc.CategoryId,
                        dc => dc.CategoryId,
                        (_, _2) => 1)
                    .Any()

                // user fits division
                where d.Gender == null || d.Gender == user.Gender
                where d.AgeLowerBound == null || d.AgeLowerBound < user.Age
                where d.AgeUpperBound == null || d.AgeUpperBound >= user.Age

                // user fits weight class
                where wc.WeightLowerBound == null || wc.WeightLowerBound <= user.Weight
                where wc.WeightUpperBound == null || wc.WeightUpperBound > user.Weight

                // selecting the board
                select b;

            return recommendations;
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
