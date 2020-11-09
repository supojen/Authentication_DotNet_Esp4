using _5.AuthorizationRequirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _5.Helper
{

    public static class CompanyAuthorizationPolicyFactory
    {
        public static AuthorizationPolicy Create(string policyName)
        {
            AuthorizationPolicy policy;

            if (policyName != null)
            {
                var splitStr = policyName.Split('.');
                var department = splitStr.First();
                if (!Int32.TryParse(splitStr.Last(), out int level))
                    throw new ArgumentException("level could not be parsed!");

                var policyBuilder = new AuthorizationPolicyBuilder();
                policy = policyBuilder.RequireAuthenticatedUser()
                                          .AddDepartmentRequirement(department)
                                          .AddlevelRequirement(level)
                                          .Build();
            }
            else
            {
                var policyBuilder = new AuthorizationPolicyBuilder();
                policy = policyBuilder.RequireAuthenticatedUser()
                                          .Build();
            }

            return policy;
        }
    }


    public class CompanyAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public CompanyAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {}

        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            return Task.FromResult(CompanyAuthorizationPolicyFactory.Create(policyName));
        }
    }
}
