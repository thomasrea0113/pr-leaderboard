using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Leaderboard.Areas.Identity.Models;
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
    }
}