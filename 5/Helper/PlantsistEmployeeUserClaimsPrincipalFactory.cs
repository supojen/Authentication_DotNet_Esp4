using _5.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace _5.Helper
{
    public class PlantsistEmployeeUserClaimsPrincipalFactory :
        UserClaimsPrincipalFactory<PlantsistEmployee>
    {
        public PlantsistEmployeeUserClaimsPrincipalFactory(
            UserManager<PlantsistEmployee> userManager, 
            IOptions<IdentityOptions> optionsAccessor) 
            : base(userManager, optionsAccessor)
        {}

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(PlantsistEmployee user)
        {
            var claimsIdentity = await base.GenerateClaimsAsync(user);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            claimsIdentity.AddClaim(new Claim("department", user.Department));
            claimsIdentity.AddClaim(new Claim("level", user.Level.ToString()));

            return claimsIdentity;
        }
    }
}
