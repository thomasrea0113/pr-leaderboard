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

        public async Task<ValueTuple<bool, ApplicationRole>> TryCreateByNameAsync(ApplicationRole role)
        {
            try
            {
                await CreateAsync(role);
                return (true, role);
            } catch (InvalidOperationException)
            {
                return (false, await FindByNameAsync(role.Name));
            }
            // var foundRole = await FindByNameAsync(role.Name);
            // if (foundRole == default)
            //     return (true, await CreateAsync(role));

            // // the passed in role may having a matching name, but the IDs may not match.
            // // We also need to overwrite the tracked entity with the passed in entity,
            // // so we detach both and update the id of the new one.
            // _ctx.Entry(foundRole).State = EntityState.Detached;
            // role.Id = foundRole.Id;
            // _ctx.Attach(role);

            // return (false, await UpdateAsync(role));
        }
    }
}