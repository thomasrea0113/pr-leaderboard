using System;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Leaderboard.Areas.Identity.Managers
{
    public class AppRoleStore : RoleStore<ApplicationRole, ApplicationDbContext, string, ApplicationUserRole, ApplicationRoleClaim>
    {
        public AppRoleStore(ApplicationDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        public async Task<IdentityResult> CreateOrFindByNameAsync(ApplicationRole role)
        {
            var newEntry = Context.Entry(role);

            if (newEntry.State != EntityState.Detached)
                throw new InvalidOperationException($"This method should only be called on an entity that is in state '{EntityState.Detached}'");

            // need to first normalize the name
            role.NormalizedName = role.Name.Normalize().ToUpper();
            var existing = await FindByNameAsync(role.NormalizedName);
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
                return await CreateAsync(role);
        }
    }
}
