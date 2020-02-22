using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public class AppUserManager : UserManager<ApplicationUser>
    {
        private readonly AppUserStore _store;

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
            _store = store as AppUserStore ??
                throw new ArgumentException($"type {store.GetType()} was not an instance of {typeof(AppUserStore)}. You must register this service first to use the app user manager.");
        }

        public async ValueTask<IdentityResult> TryAddToRoleAsync(ApplicationUser user, string role)
        {
            if (!await IsInRoleAsync(user, role))
            {
                return await AddToRoleAsync(user, role);
            }
            return default;
        }

        private string GetIdClaim(ClaimsPrincipal identity) => identity.FindFirst(ClaimTypes.NameIdentifier).Value;

        /// <summary>
        /// Gets the user, but also loads the properties specified by include
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="Func<IQueryable<ApplicationUser>"></param>
        /// <param name="include"></param>
        /// <typeparam name="TProp"></typeparam>
        /// <returns></returns>
        public async Task<ApplicationUser> GetUserAsync<TProp>(ClaimsPrincipal identity,
            Expression<Func<IQueryable<ApplicationUser>, IIncludableQueryable<ApplicationUser, TProp>>> include)
            => await GetUserAsync(GetIdClaim(identity), include);

        public async Task<ApplicationUser> GetUserAsync<TProp>(string id,
            Expression<Func<IQueryable<ApplicationUser>, IIncludableQueryable<ApplicationUser, TProp>>> include)
            => await include.Compile()(Users).FirstOrDefaultAsync(u => u.Id == id);

        public async Task<ApplicationUser> GetUserAsync<TProp>(Expression<Func<ApplicationUser, bool>> equality,
            Expression<Func<IQueryable<ApplicationUser>, IIncludableQueryable<ApplicationUser, TProp>>> include)
            => await include.Compile()(Users).FirstOrDefaultAsync(equality);

        private readonly Expression<Func<IQueryable<ApplicationUser>, IIncludableQueryable<ApplicationUser, LeaderboardModel>>> _allNavProps =
            user => user
                .Include(u => u.UserCategories).ThenInclude(uc => uc.Category)
                .Include(u => u.UserLeaderboards).ThenInclude(ul => ul.Leaderboard);

        /// <summary>
        /// Get the user, and also load all of the relavent navigation properties
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> GetCompleteUser(ClaimsPrincipal identity)
            => await GetCompleteUser(GetIdClaim(identity));

        public async Task<ApplicationUser> GetCompleteUser(string id)
            => await GetUserAsync(id, _allNavProps);

        public async Task<ApplicationUser> GetCompleteUser(Expression<Func<ApplicationUser, bool>> equality)
            => await GetUserAsync(equality, _allNavProps);
        /// <summary>
        /// Joins all the divisions on the users categories. This will not filter out
        /// divisions that the user is already a part of
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<List<LeaderboardModel>> GetRecommendedBoardsAsync(ApplicationUser user)
            => await _store.GetRecommendedBoardsAsync(user);

        public async Task<IdentityResult> CreateOrUpdateByNameAsync(ApplicationUser user, string password)
        {
            var created = await _store.CreateOrFindByIdAsync(user);
            await ResetPasswordAsync(user, await GeneratePasswordResetTokenAsync(user), password);
            return created;
        }
    }
}