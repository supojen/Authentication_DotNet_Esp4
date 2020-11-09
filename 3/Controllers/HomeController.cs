using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _1.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace _1.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<PlantsistEmployee> _userManager;
        private readonly IUserClaimsPrincipalFactory<PlantsistEmployee> _claimsPrincipalFactory;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            UserManager<PlantsistEmployee> userManager,
            IUserClaimsPrincipalFactory<PlantsistEmployee> claimsPrincipalFactory,
            IAuthorizationService authorizationService,
            ILogger<HomeController> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _claimsPrincipalFactory = claimsPrincipalFactory ?? throw new ArgumentNullException(nameof(claimsPrincipalFactory));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Secret()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                if (await _userManager.CheckPasswordAsync(user, password))
                {
                    var claimsPrincipal = await _claimsPrincipalFactory.CreateAsync(user);
                    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);
                    return RedirectToAction("Secret");
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                user = new PlantsistEmployee
                {
                    UserName = username,
                    Department = "Technology",
                    level = 1
                };

                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    var claimsPrincipal = await _claimsPrincipalFactory.CreateAsync(user);
                    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);
                    return RedirectToAction("Secret");
                }
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> UseAuthorizationService1()
        {
            var authoResult = await _authorizationService.AuthorizeAsync(HttpContext.User, "Technology");
            if (authoResult.Succeeded)
            {
                _logger.LogWarning("The user has been authorized!");
            }
            else
            {
                _logger.LogWarning("The user has not been authorized!");
            }
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> UseAuthorizationService2()
        {
            var policyBuilder = new AuthorizationPolicyBuilder();
            var policy = policyBuilder.RequireAuthenticatedUser().RequireClaim("age").Build();

            var authoResult = await _authorizationService.AuthorizeAsync(HttpContext.User, policy);
            if (authoResult.Succeeded)
            {
                _logger.LogWarning("The user has been authorized!");
            }
            else
            {
                _logger.LogWarning("The user has not been authorized!");
            }
            return RedirectToAction("Index");
        }
    }
}
