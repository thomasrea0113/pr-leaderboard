using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Leaderboard.Data;
using Leaderboard.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Leaderboard.Managers
{
    public class UserProfileManager : UserManager<IdentityUser<Guid>>
    {
        private readonly ApplicationDbContext _dbCtx;

        public UserProfileManager(IUserStore<IdentityUser<Guid>> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<IdentityUser<Guid>> passwordHasher,
            IEnumerable<IUserValidator<IdentityUser<Guid>>> userValidators,
            IEnumerable<IPasswordValidator<IdentityUser<Guid>>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<IdentityUser<Guid>>> logger) : base(store, optionsAccessor,
                passwordHasher, userValidators, passwordValidators, keyNormalizer,
                errors, services, logger)
        {
            _dbCtx = services.GetRequiredService<ApplicationDbContext>();
        }

        /// <summary>
        /// Get or creates the user's profile. Returns a tuple indicating if the user profile was created or retrieved
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<ValueTuple<bool, UserProfileModel>> GetOrCreateProfileAsync(IdentityUser<Guid> user)
            => await GetOrCreateProfileAsync(user.Id);

        public async Task<ValueTuple<bool, UserProfileModel>> GetOrCreateProfileAsync(Guid userId)
        {
            var profile = await _dbCtx.UserProfiles.FindAsync(userId);
            if (profile == default)
            {
                var result = await _dbCtx.UserProfiles.AddAsync( new UserProfileModel { UserId = userId });
                return (true, result.Entity);
            }
            else return (false, profile);
        }
    }
}