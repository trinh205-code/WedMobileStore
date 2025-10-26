using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebMobileStore.Models.Data;
using WebMobileStore.Models.Entity;
using WebMobileStore.Models.Entity.Enums;

namespace WebMobileStore.Controllers
{
    [Route("/User")]
    public class UserController : Controller
    {
        private readonly MobileStoreContext db;

        public UserController(MobileStoreContext context)
        {
            db = context;
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.role.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );

                return RedirectToAction("Index", "Home");
            }

            ViewBag.LoginError = "Email hoặc mật khẩu không đúng";
            return View();
        }


        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        public IActionResult Create(Users model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (db.Users.Any(u => u.Email == model.Email))
            {
                ViewBag.Error = "Email đã tồn tại!";
                return View(model);
            }

            model.role = Role.Customer;
            db.Users.Add(model);

            var cart = new Carts
            {
                UserId = model.UserId,
            };

            db.Carts.Add(cart);
            db.SaveChanges();

            return RedirectToAction("Login");
        }



        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        

    }
}
