using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMobileStore.Models.Data;
using WebMobileStore.Models.Entity;
using WebMobileStore.Models.Entity.Enums;
using WebMobileStore.ViewModels;

namespace WebMobileStore.Controllers
{
    [Route("checkout")]
    public class CheckOutController : Controller
    {
        private readonly MobileStoreContext db;
        public CheckOutController(MobileStoreContext context)
        {
            db = context;
        }

        // GET /checkout
        [HttpGet("")]
        public IActionResult Index()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null) return RedirectToAction("Login", "User");

            var userId = long.Parse(userIdClaim.Value);

            var user = db.Users
              .Include(u => u.Address)
              .Include(u => u.Carts)
                  .ThenInclude(c => c.Items)
                      .ThenInclude(ci => ci.ProductVariant)
                          .ThenInclude(pv => pv.Products)
                              .ThenInclude(p => p.ProductImages) 
              .FirstOrDefault(u => u.UserId == userId);


            if (user == null) return RedirectToAction("Index", "Shop");

            var cartItems = user.Carts?.Items?.ToList() ?? new List<CartItem>();

            var model = new CheckoutViewModel
            {
                User = user,
                CartItems = cartItems,
                ShippingFee = 30000
            };

            return View(model); 
        }

        // POST /checkout/place-order
        [HttpPost("placeorder")]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(CheckoutViewModel model)
        {
            if (!ModelState.IsValid) return View("Index", model);

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null) return RedirectToAction("Login", "User");

            var userId = long.Parse(userIdClaim.Value);

            var cartItems = model.CartItems ?? new List<CartItem>();
            if (!cartItems.Any())
            {
                ModelState.AddModelError("", "Giỏ hàng trống.");
                return View("Index", model);
            }

            var order = new Orders
            {
                UserId = userId,
                Address = model.Address,
                Ward = model.Ward,
                District = model.District,
                City = model.City,
                ShippingFee = model.ShippingFee,
                TotalAmount = cartItems.Sum(i => i.Quantity * i.ProductVariant.Price) + model.ShippingFee,
                OrderdAt = DateTime.Now,
                OrderStatus = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                OrderDetails = cartItems.Select(ci => new OrderDetail
                {
                    ProductName = ci.ProductVariant.Products.ProductsName,
                    Color = ci.ProductVariant.Color,
                    Storage = ci.ProductVariant.Storage,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.ProductVariant.Price,
                    TotalPrice = ci.Quantity * ci.ProductVariant.Price,
                    ProductVariantId = ci.ProductVariantId
                }).ToList()
            };

            db.Orders.Add(order);
            db.SaveChanges();

            // Xóa cart sau khi đặt hàng
            var cart = db.Carts.Include(c => c.Items).FirstOrDefault(c => c.UserId == userId);
            if (cart != null && cart.Items.Any())
            {
                db.CartItems.RemoveRange(cart.Items);
                db.SaveChanges();
            }

            return RedirectToAction("Success", new { orderId = order.OrdersId });
        }

        // GET /checkout/success?orderId=123
        [HttpGet("success")]
        public IActionResult Success(long orderId)
        {
            var order = db.Orders
                          .Include(o => o.OrderDetails)
                          .FirstOrDefault(o => o.OrdersId == orderId);
            if (order == null) return RedirectToAction("Index", "Shop");

            return View(order);
        }
    }
}
