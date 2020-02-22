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
        private readonly AppRoleStore _store;

        public AppRoleManager(IRoleStore<ApplicationRole> store,
            IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            ILogger<RoleManager<ApplicationRole>> logger) :
                base(store, roleValidators, keyNormalizer, errors, logger)
        {
            _store = store as AppRoleStore ??
                throw new ArgumentException($"type {store.GetType()} was not an instance of {typeof(AppRoleStore)}. You must register this service first to use the app role manager.");
        }

        public async Task<IdentityResult> TryCreateByNameAsync(ApplicationRole role) => await _store.CreateOrFindByNameAsync(role);
    }
}