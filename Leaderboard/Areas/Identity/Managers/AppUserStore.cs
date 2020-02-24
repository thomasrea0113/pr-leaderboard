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
        public async Task<List<LeaderboardModel>> GetRecommendedBoardsAsync(ApplicationUser user)
        {
            // TODO Should I join on the user table rather than explicitly passing the weight/age/id values?
            // LINQ to Queries is quite powerful! The key to remember is to NEVER use a navigation
            // property in your query. Always use the DbSet on the context, and then use joins
            // var recommendations = from b in Context.Leaderboards.AsQueryable()
            //     join d in Context.Divisions on dc.DivisionId equals d.Id
            //     join dwc in Context.DivisionWeightClasses on d.Id equals dwc.DivisionId
            //     join wc in Context.WeightClasses on dwc.WeightClassId equals wc.Id

            //     // getting only divisions that overlap with the users categories
            //     (
            //             (d.Gender == null || user.Gender == d.Gender) &&
            //             (d.AgeLowerBound == null || d.AgeLowerBound < user.Age) &&
            //             (d.AgeUpperBound == null || user.Age <= d.AgeUpperBound)
            //         ) &&

            //         // if the board has no weight class, the user automatically qualifies
            //         (
            //             b.WeightClassId == null ||
            //             (
            //                 (wc.WeightLowerBound == null || wc.WeightLowerBound <= user.Weight) &&
            //                 (wc.WeightUpperBound == null || user.Weight < wc.WeightUpperBound)
            //             )
            //         )

            //     select b;

            var recommendations = Context.Leaderboards.AsQueryable()
                .Where(b => b.IsActive == true)
                .Join(Context.Divisions, b => b.DivisionId, d => d.Id, (b, d) => new { b, d })
                .Where(bd => Context.UserCategories.AsQueryable().Where(uc => uc.UserId == user.Id)
                    .Join(Context.DivisionCategories.AsQueryable().Where(dc => dc.DivisionId == bd.d.Id),
                        uc => uc.CategoryId,
                        dc => dc.CategoryId,
                        (_, _2) => 1)
                    .Any()
                )
                .Where(bd => bd.d.Gender == null || bd.d.Gender == user.Gender)
                .Where(bd =>
                    !Context.DivisionWeightClasses.AsQueryable()
                        .Where(dwc => dwc.DivisionId == bd.d.Id).Any() ||
                    Context.DivisionWeightClasses.AsQueryable()
                        .Where(dwc => dwc.DivisionId == bd.d.Id)
                        .Join(Context.WeightClasses, dwc => dwc.WeightClassId, wc => wc.Id, (_, wc) => wc)
                        .Where(wc => wc.WeightLowerBound == null || wc.WeightLowerBound <= user.Weight)
                        .Where(wc => wc.WeightUpperBound == null || wc.WeightUpperBound > user.Weight)
                        .Any()
                )
                .Select(bd => bd.b);
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
