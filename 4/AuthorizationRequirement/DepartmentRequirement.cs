using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _1.AuthorizationRequirement
{
    public class DepartmentRequirement : IAuthorizationRequirement  
    {
        public string Department { get; set; }
        public DepartmentRequirement(string department)
        {
            Department = department;
        }
    }

    public class DepartmentrequirementHandler : AuthorizationHandler<DepartmentRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, DepartmentRequirement requirement)
        {
            var departmentClaim = context.User.Claims.Where(x => x.Type == "department").SingleOrDefault();
            if(departmentClaim != null)
            {
                if(departmentClaim.Value == requirement.Department)
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
