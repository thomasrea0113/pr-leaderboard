using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Identity.ViewModels;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Leaderboard.Areas.Identity.Managers
{
    public static class UserManagerExtensions
    {
        public static async ValueTask<IdentityResult> TryAddToRoleAsync<TUser>(this UserManager<TUser> manager, TUser user, string role)
            where TUser : IdentityUser
        {
            if (!await manager.IsInRoleAsync(user, role))
            {
                return await manager.AddToRoleAsync(user, role);
            }
            return default;
        }
    }

    public class AppUserManager : UserManager<ApplicationUser>
    {
        private readonly ApplicationDbContext _ctx;

        public AppUserManager(IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger) :
                base(store, optionsAccessor, passwordHasher, userValidators,
                passwordValidators, keyNormalizer, errors, services, logger)
        {
            _ctx = services.GetRequiredService<ApplicationDbContext>();
        }

        /// <summary>
        /// Gets the user, but also loads the properties specified by include
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="Func<IQueryable<ApplicationUser>"></param>
        /// <param name="include"></param>
        /// <typeparam name="TProp"></typeparam>
        /// <returns></returns>
        public async Task<ApplicationUser> GetUserAsync<TProp>(ClaimsPrincipal identity,
            Func<IQueryable<ApplicationUser>, IIncludableQueryable<ApplicationUser, TProp>> include)
        {
            // will be the user id
            var id = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            return await include(Users).SingleAsync(u => u.Id == id);
        }

        /// <summary>
        /// Get the user, and also load all of the relavent navigation properties
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> GetCompleteUser(ClaimsPrincipal identity)
            => await GetUserAsync(identity, user => user
                .Include(u => u.UserCategories).ThenInclude(uc => uc.Category)
                .Include(u => u.UserLeaderboards).ThenInclude(ul => ul.Leaderboard));

        /// <summary>
        /// Joins all the divisions on the users categories. This will not filter out
        /// divisions that the user is already a part of
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<List<LeaderboardModel>> GetRecommendedBoardsAsync(ApplicationUser user)
        {
            var userAge = user.Age;
            var userWeight = user.Weight;
            var userGender = user.Gender;

            // LINQ to Queries is quite powerful! The key to remember is to NEVER use a navigation
            // property in your query. Always use the DbSet on the context, and then use joins
            var recommendations = from dc in _ctx.DivisionCategories.AsQueryable()
                from b in _ctx.Leaderboards
                from d in _ctx.Divisions
                from wc in _ctx.WeightClasses
                join uc in _ctx.UserCategories on dc.CategoryId equals uc.CategoryId
                where uc.UserId == user.Id
                where d.AgeLowerBound < userAge && userAge <= d.AgeUpperBound
                where d.Gender == userGender
                where wc.WeightLowerBound < userWeight && userWeight <= wc.WeightUpperBound
                select b;
            return await recommendations.ToListAsync();
        }

        public async Task<bool> CreateOrUpdateByNameAsync(ApplicationUser user, string password)
        {
            user.NormalizedUserName = user.UserName.ToUpper();
            var existingUser = await FindByNameAsync(user.NormalizedUserName);
            if (existingUser == default)
            {
                await CreateAsync(user, password);
                return true;
            }

            // the passed in user may having a matching name, but the IDs may not match.
            // We also need to overwrite the tracked entity with the passed in entity,
            // so we detach both and update the id of the new one.
            user.Id = existingUser.Id;

            var tracked = _ctx.ChangeTracker.Entries()
                .Where(e => e.Entity is ApplicationUser)
                .SingleOrDefault(e => (e.Entity as ApplicationUser)?.Id == user.Id);
                
            // if we're already tracking this entity, then we simply need to overwrite the values
            if (tracked != default)
            {
                tracked.CurrentValues.SetValues(user);
                await _ctx.SaveChangesAsync();
                var trackedEntity = (ApplicationUser)tracked.Entity;
                await ResetPasswordAsync(trackedEntity, await GeneratePasswordResetTokenAsync(trackedEntity), password);
            }
            else
            {
                await UpdateAsync(user);
                await ResetPasswordAsync(user, await GeneratePasswordResetTokenAsync(user), password);
            }

            return false;
        }
    }
}