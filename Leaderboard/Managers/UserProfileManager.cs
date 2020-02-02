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
    public class UserProfileManager : UserManager<IdentityUser<Guid>>
    {
        private ApplicationDbContext _ctx { get; }

        public UserProfileManager(IUserStore<IdentityUser<Guid>> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<IdentityUser<Guid>> passwordHasher,
            IEnumerable<IUserValidator<IdentityUser<Guid>>> userValidators,
            IEnumerable<IPasswordValidator<IdentityUser<Guid>>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            IServiceProvider services, ILogger<UserManager<IdentityUser<Guid>>> logger) :
                base(store, optionsAccessor, passwordHasher, userValidators,
                passwordValidators, keyNormalizer, errors, services, logger)
        {
            _ctx = services.GetRequiredService<ApplicationDbContext>();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _ctx.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserProfileModel> GetProfileAsync(IdentityUser<Guid> user)
            => await GetProfileAsync(user.Id);

        public async Task<UserProfileModel> GetProfileAsync(Guid userId)
        {
            var profile = await _ctx.UserProfiles.FindAsync(userId);
            await _ctx.Entry(profile).ReloadAsync();
            return profile;
        }
    }
}