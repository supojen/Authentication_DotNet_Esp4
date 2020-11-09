using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _1.AuthorizationRequirement
{
    public class CustomClaimRequirement : IAuthorizationRequirement
    {
        public string ClaimType { get; set; }   
        public CustomClaimRequirement(string claimType) 
        {
            ClaimType = claimType;
        }
    }

    public class CustomClaimRequirementHandler :
        AuthorizationHandler<CustomClaimRequirement>
    {
        private readonly ILogger<CustomClaimRequirementHandler> _logger;

        public CustomClaimRequirementHandler(ILogger<CustomClaimRequirementHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, CustomClaimRequirement requirement)
        {
            _logger.LogError("Hello World");

            var hasClaim = context.User.Claims.Any(x => x.Type == requirement.ClaimType);
            if(hasClaim)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

}
