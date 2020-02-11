using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> CreateOrUpdateByNameAsync(ApplicationUser user, string password)
        {
            var existingUser = await FindByNameAsync(user.UserName);
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