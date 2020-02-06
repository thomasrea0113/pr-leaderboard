using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Profiles.Models;
using Leaderboard.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Leaderboard.Areas.Identity.Managers
{
    public class AppUserManager : UserManager<ApplicationUser>
    {
        private ApplicationDbContext _ctx { get; }

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

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _ctx.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserProfileModel> GetProfileAsync(ApplicationUser user)
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