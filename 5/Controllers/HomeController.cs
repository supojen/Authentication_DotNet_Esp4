using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _5.ActionAttribute;
using _5.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _5.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<PlantsistEmployee> _userManager;
        private readonly IUserClaimsPrincipalFactory<PlantsistEmployee> _claimsPrincipalFactory;

        public HomeController(
            UserManager<PlantsistEmployee> userManager,
             IUserClaimsPrincipalFactory<PlantsistEmployee> claimsPrincipalFactory)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _claimsPrincipalFactory = claimsPrincipalFactory ?? throw new ArgumentNullException(nameof(claimsPrincipalFactory));
        }

        public IActionResult Index()
        {
            return View();
        }

        [CompanyAuthorize("Technology",2)]
        public IActionResult Secret()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
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
            if(user != null)
            {
                if(await _userManager.CheckPasswordAsync(user,password))
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
            if(user == null)
            {
                user = new PlantsistEmployee
                {
                    UserName = username,
                    Email = "brian71742@gmail.com",
                    Department = "Technology",
                    Level = 1
                };
                var createResult = await _userManager.CreateAsync(user, password);
                if(createResult.Succeeded)
                {
                    var claimsPrincipal = await _claimsPrincipalFactory.CreateAsync(user);
                    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);
                    return RedirectToAction("Secret");
                }
            }

            return RedirectToAction("Index");
        }

    }
}
