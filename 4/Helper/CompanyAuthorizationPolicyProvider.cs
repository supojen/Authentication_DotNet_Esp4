using _1.AuthorizationRequirement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _1.Helper
{
    public class CompanyAuthorizeAttribute : AuthorizeAttribute
    {
        public CompanyAuthorizeAttribute(string department, int level)
        {
            Policy = $"{department}.{level}";
        }
    }

    public static class CompanyPolicyFactory
    {
        public static AuthorizationPolicy Create(string policyName)
        {
            if(policyName != null)
            {
                var split = policyName.Split('.');
                var department = split[0];
                if (!Int32.TryParse(split[1], out int level))
                    throw new ArgumentException("Level cannot be translated to integer.");

                return new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .AddRequirements(new DepartmentRequirement(department))
                                .AddRequirements(new LevelRequirement(level))
                                .Build();
            }
            else
            {
                return new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
            }
        }
    }


    public class CompanyAuthorizationPolicyProvider
        : DefaultAuthorizationPolicyProvider
    {
        public CompanyAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {}

        // {department}.{level}
        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            return Task.FromResult(CompanyPolicyFactory.Create(policyName));
        }
    }
}
