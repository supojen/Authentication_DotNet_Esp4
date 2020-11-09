using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace _1.Controllers
{
    [Route("OperationRequirement")]
    public class OperationsController1 : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<OperationsController1> _logger;

        public OperationsController1(
            IAuthorizationService authorizationService,
            ILogger<OperationsController1> logger
            )
        {
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Route("Technology")]
        public async Task<IActionResult> TechnologyAdminOpration()
        {
            var policyBuilder = new AuthorizationPolicyBuilder();
            var policy = policyBuilder.RequireAuthenticatedUser()
                                      .AddRequirements(GetOperationRequirement.Technology)
                                      .Build();


            if((await _authorizationService.AuthorizeAsync(HttpContext.User, 1, policy)).Succeeded)
            {
                _logger.LogWarning("Do Something which only the admin in technology department can do");
            }
            else
            {
                _logger.LogWarning("Other than technology admin");
            }

            return RedirectToAction("Index", "Home");
        }
    }


    public class DepartmentAuthorizationHandler :
        AuthorizationHandler<OperationAuthorizationRequirement,int>
    {
        private readonly ILogger<DepartmentAuthorizationHandler> _logger;

        public DepartmentAuthorizationHandler(ILogger<DepartmentAuthorizationHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement,
            int level)
        {
            var departmentClaim = context.User.Claims.Where(x => x.Type == "department").SingleOrDefault();
            if (departmentClaim == null)
                throw new ArgumentNullException(nameof(departmentClaim));
            var department = departmentClaim.Value;

            _logger.LogWarning($"Department = {department}");  // Debug

            switch(department)
            {
                case DepartmentOprations.Technology:
                    if (requirement.Name == DepartmentOprations.Technology)
                        context.Succeed(requirement);
                    break;
                case DepartmentOprations.Finance:
                    if (requirement.Name == DepartmentOprations.Finance)
                        context.Succeed(requirement);
                    break;
                case DepartmentOprations.Operation:
                    if (requirement.Name == DepartmentOprations.Operation)
                        context.Succeed(requirement);
                    break;
                default:
                    break;
            }

            return Task.CompletedTask;
        }
    }

    public static class DepartmentOprations
    {
        public const string Technology = "Technology";
        public const string Finance = "Finance";
        public const string Operation = "Operation";
    }


    public static class GetOperationRequirement     
    {
        public static OperationAuthorizationRequirement Technology =>
            new OperationAuthorizationRequirement
            {
                Name = DepartmentOprations.Technology
            };

        public static OperationAuthorizationRequirement Finance =>
            new OperationAuthorizationRequirement
            {
                Name = DepartmentOprations.Finance
            };

        public static OperationAuthorizationRequirement Operation =>
            new OperationAuthorizationRequirement
            {
                Name = DepartmentOprations.Operation
            };
    }
}
