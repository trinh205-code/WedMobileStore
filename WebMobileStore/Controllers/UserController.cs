using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebMobileStore.Models.Data;
using WebMobileStore.Models.Entity;
using WebMobileStore.Models.Entity.Enums;
using WebMobileStore.Models;

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
                    new Claim("UserId", user.UserId.ToString()), // Bắt buộc cho checkout
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
            db.SaveChanges();

            var cart = new Carts
            {
                UserId = model.UserId
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

        [HttpGet("Profile")]
        public IActionResult Profile()
        {
            var userId = User.FindFirstValue("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = db.Users
                .Where(u => u.UserId == long.Parse(userId))
                .FirstOrDefault();

            if (user == null)
                return RedirectToAction("Login");

            return View(user); // ✅ Trả về đúng kiểu Model là Users
        }

        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword()
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost("ChangePassword")]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = db.Users.FirstOrDefault(u => u.UserId == long.Parse(userId));
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Kiểm tra mật khẩu hiện tại
            if (user.Password != model.CurrentPassword)
            {
                TempData["Error"] = "Mật khẩu hiện tại không chính xác";
                return View(model);
            }

            // Kiểm tra mật khẩu mới không trùng mật khẩu cũ
            if (model.NewPassword == model.CurrentPassword)
            {
                TempData["Error"] = "Mật khẩu mới không được trùng với mật khẩu hiện tại";
                return View(model);
            }

            // Cập nhật mật khẩu mới
            user.Password = model.NewPassword;

            db.Users.Update(user);
            db.SaveChanges();

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Profile");
        }

        // Thêm vào UserController.cs (sau action ChangePassword hiện tại)

        [HttpGet("EditProfile")]
        public IActionResult EditProfile()
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = db.Users.FirstOrDefault(u => u.UserId == long.Parse(userId));
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new EditProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.Phone,
                Address = user.Address?.ToString()

            };

            return View(model);
        }

        [HttpPost("EditProfile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = db.Users.FirstOrDefault(u => u.UserId == long.Parse(userId));
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Kiểm tra mật khẩu hiện tại
            if (user.Password != model.CurrentPassword)
            {
                TempData["Error"] = "Mật khẩu hiện tại không chính xác";
                return View(model);
            }

            // Kiểm tra email mới có trùng với người dùng khác không
            if (model.Email != user.Email)
            {
                var existingUser = db.Users.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser != null && existingUser.UserId != user.UserId)
                {
                    TempData["Error"] = "Email này đã được sử dụng bởi tài khoản khác";
                    return View(model);
                }
            }

            // Cập nhật thông tin
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Phone = model.PhoneNumber;
            if (user.Address == null)
                user.Address = new Address();

            var parts = model.Address?.Split(',') ?? new string[0];
            user.Address.Ward = parts.Length > 0 ? parts[0].Trim() : "";
            user.Address.District = parts.Length > 1 ? parts[1].Trim() : "";
            user.Address.City = parts.Length > 2 ? parts[2].Trim() : "";



            db.Users.Update(user);
            db.SaveChanges();

            TempData["Success"] = "Cập nhật thông tin thành công!";

            // Nếu email thay đổi, cần đăng nhập lại để cập nhật claims
            if (model.Email != User.FindFirstValue(ClaimTypes.Email))
            {
                await HttpContext.SignOutAsync();

                var claims = new List<Claim>
        {
            new Claim("UserId", user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.role.ToString())
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );
            }

            return RedirectToAction("Profile");
        }

    }
}