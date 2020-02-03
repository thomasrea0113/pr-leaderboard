using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Leaderboard.Data;
using Leaderboard.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Leaderboard.Managers
{
    public class UserProfileManager : UserManager<IdentityUser>
    {
        private ApplicationDbContext _ctx { get; }

        public UserProfileManager(IUserStore<IdentityUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<IdentityUser> passwordHasher,
            IEnumerable<IUserValidator<IdentityUser>> userValidators,
            IEnumerable<IPasswordValidator<IdentityUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            IServiceProvider services, ILogger<UserManager<IdentityUser>> logger) :
                base(store, optionsAccessor, passwordHasher, userValidators,
                passwordValidators, keyNormalizer, errors, services, logger)
        {
            _ctx = services.GetRequiredService<ApplicationDbContext>();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _ctx.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserProfileModel> GetProfileAsync(IdentityUser user)
            => await GetProfileAsync(user.Id);

        public async Task<UserProfileModel> GetProfileAsync(string userId)
        {
            var profile = await _ctx.UserProfiles.FindAsync(userId);

            // becauze lazy loading requries both entities be aware of the relationship,
            // it won't work. We have to explicitly load the object here.
            profile.User = await FindByIdAsync(userId.ToString());

            return profile;
        }
    }
}