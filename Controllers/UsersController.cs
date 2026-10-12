using Microsoft.AspNetCore.Mvc;
using WebAppMVC.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using WebAppMVC.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebAppMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly WebAppMVCContext _context;

        public UsersController(WebAppMVCContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, string? returnUrl = null)
        {
            if(ModelState.IsValid)
            {
                //Validate Username
                var user = _context.Customer.Where(c => c.Email == model.Username).FirstOrDefault();
                if(user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    return View(model);
                }
                // Add claims for the user
                // Claims are pieces of information about the user that can be used for authorization and authentication purposes.
                // In this case, we are adding claims for the user's name, email, country, and role.
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Country, user.Country),
                    new Claim(ClaimTypes.Role, "Customer")
                };

                // Create the identity and principal
                /* We needed to create claims firstly because the identity is created from the claims.
                 * The identity represents the user and their claims, while the principal represents the user and their identity.
                 * The principal can have multiple identities, but in this case, we are only creating one identity for the user.*/
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Sign in the user
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                // Check if the Url is not Null or Empty
                if(!string.IsNullOrEmpty(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                // If it's Empty/Null redirect to the Home page
                return RedirectToAction("index", "Home");
            }
            return View(model);
        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("index", "Home");
        }
    }
}
