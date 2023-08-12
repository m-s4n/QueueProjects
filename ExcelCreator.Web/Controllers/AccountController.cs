using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExcelCreator.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager; 
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Login()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var hasUser = await _userManager.FindByEmailAsync(Email);
            if (hasUser is null) return View();
            var signInResullt = await _signInManager
                .PasswordSignInAsync(
                user: hasUser, 
                password: Password, 
                isPersistent: true, 
                lockoutOnFailure: false);
            if (!signInResullt.Succeeded) return View();

            // Login basarili homepage'a yonlendirilir

            return RedirectToAction(nameof(HomeController.Index),"Home");
        }
    }
}
