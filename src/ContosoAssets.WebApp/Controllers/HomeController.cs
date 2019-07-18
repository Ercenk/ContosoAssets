using System.Diagnostics;
using ContosoAssets.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContosoAssets.WebApp.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;

        public HomeController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier});
        }

        public IActionResult Index()
        {
            return this.View();
        }
    }
}
