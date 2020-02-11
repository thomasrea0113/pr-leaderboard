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

    public class AppRoleManager : RoleManager<ApplicationRole>
    {
        private ApplicationDbContext _ctx { get; }

        public AppRoleManager(IRoleStore<ApplicationRole> store,
            IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            ILogger<RoleManager<ApplicationRole>> logger, ApplicationDbContext context) :
                base(store, roleValidators, keyNormalizer, errors, logger)
        {
            _ctx = context;
        }

        public async Task<bool> TryCreateByNameAsync(ApplicationRole role)
        {
            role.NormalizedName = role.Name.ToUpper();
            var existingRole = await FindByNameAsync(role.NormalizedName);
            if (existingRole == default)
            {
                await CreateAsync(role);
                return true;
            }

            // the passed in user may having a matching name, but the IDs may not match.
            // We also need to overwrite the tracked entity with the passed in entity,
            // so we detach both and update the id of the new one.
            role.Id = existingRole.Id;

            var tracked = _ctx.ChangeTracker.Entries()
                .Where(e => e.Entity is ApplicationRole)
                .SingleOrDefault(e => (e.Entity as ApplicationRole)?.Id == role.Id);

            // if we're already tracking this entity, then we simply need to overwrite the values
            if (tracked != default)
            {
                tracked.CurrentValues.SetValues(role);
                await _ctx.SaveChangesAsync();
            }
            else
                await UpdateAsync(role);
            return false;
        }
    }
}