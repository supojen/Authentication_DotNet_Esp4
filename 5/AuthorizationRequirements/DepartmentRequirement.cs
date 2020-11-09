using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _5.AuthorizationRequirements
{
    public class DepartmentRequirement : IAuthorizationRequirement
    {
        public string Department { get; set; }
        public DepartmentRequirement(string department)
        {
            Department = department;
        }
    }

    public class DepartmentRequirementHandler :
        AuthorizationHandler<DepartmentRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, DepartmentRequirement requirement)
        {
            var departmentClaim = context.User.Claims.Where(x => x.Type == "department").SingleOrDefault();
            if (departmentClaim == null)
                throw new ArgumentNullException(nameof(departmentClaim));

            if (departmentClaim.Value == requirement.Department)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }


    public static class AuthorizationPolicyBuilderexetension
    {
        public static AuthorizationPolicyBuilder AddDepartmentRequirement(
            this AuthorizationPolicyBuilder builder, string department)
        {
            builder.AddRequirements(new DepartmentRequirement(department));

            return builder;
        }
    }

}
