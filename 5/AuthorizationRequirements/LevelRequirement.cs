using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace _5.AuthorizationRequirements
{
    public class LevelRequirement : IAuthorizationRequirement
    {
        public int Level { get; set; }
        public LevelRequirement(int level)
        {
            Level = level;
        }
    }

    public class LevelRequirementHandler : AuthorizationHandler<LevelRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, LevelRequirement requirement)
        {
            var levelClaim = context.User.Claims.Where(x => x.Type == "level").SingleOrDefault();
            if (levelClaim == null)
                throw new ArgumentNullException(nameof(levelClaim));

            if(Int32.TryParse(levelClaim.Value, out int level))
            {
                if (level <= requirement.Level)
                    context.Succeed(requirement);
            }
            else
            {
                throw new ArgumentException("level could not be pasred!");
            }

            return Task.CompletedTask;
        }
    }


    public static class AuthorizationPolicyBuilderExetension    
    {
        public static AuthorizationPolicyBuilder AddlevelRequirement(
            this AuthorizationPolicyBuilder builder, int level)
        {
            builder.AddRequirements(new LevelRequirement(level));

            return builder;
        }
    }
}
