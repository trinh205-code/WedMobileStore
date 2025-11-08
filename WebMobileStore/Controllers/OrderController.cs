using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMobileStore.Models.Data;
using WebMobileStore.Models.Entity;
using WebMobileStore.Models.Entity.Enums;
using WebMobileStore.ViewModels;

namespace WebMobileStore.Controllers
{
    [Route("Order")]
    public class OrderController : Controller
    {
        private readonly MobileStoreContext db;
        public OrderController(MobileStoreContext context)
        {
            db = context;
        }

        // GET /checkout
        [HttpGet("checkout")]
        public IActionResult checkout()
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

            var cartItems = user.Carts?.Items?.Where(ci => ci.ProductVariant != null)?.ToList() ?? new List<CartItem>();

            var model = new CheckoutViewModel
            {
                User = user,
                CartItems = cartItems,
                ShippingFee = 30000,

                FullName = user.FullName,
                Phone = user.Phone,
                Email = user.Email,

                Ward = user.Address?.Ward ?? "",
                District = user.Address?.District ?? "",
                City = user.Address?.City ?? ""
            };

            return View("Index", model);
        }

        [HttpPost("placeorder")]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(CheckoutViewModel model)
        {
            try
            {
                ModelState.Remove("User");
                ModelState.Remove("CartItems");
          

                var currentUserIdClaim = User.FindFirst("UserId");
                if (currentUserIdClaim == null)
                {
                    TempData["ErrorMessage"] = "Bạn chưa đăng nhập.";
                    return RedirectToAction("Login", "User");
                }

                var currentUserId = long.Parse(currentUserIdClaim.Value);

                var currentUser = db.Users
                    .Include(u => u.Address)
                    .Include(u => u.Carts)
                        .ThenInclude(c => c.Items)
                            .ThenInclude(i => i.ProductVariant)
                                .ThenInclude(pv => pv.Products)
                    .FirstOrDefault(u => u.UserId == currentUserId);

                if (currentUser == null)
                {
                    TempData["ErrorMessage"] = "Người dùng không tồn tại.";
                    return RedirectToAction("Index", "Shop");
                }

                // Filter cartItems
                var cartItems = currentUser.Carts?.Items?.Where(i => i.ProductVariant != null)?.ToList() ?? new List<CartItem>();
                if (!cartItems.Any())
                {
                    TempData["ErrorMessage"] = "Giỏ hàng trống hoặc có sản phẩm không hợp lệ.";
                    model.User = currentUser;
                    model.CartItems = cartItems;
                    return View("Index", model);
                }

                // Kiểm tra stock
                foreach (var item in cartItems)
                {
                    if (item.Quantity > item.ProductVariant.Quantity)
                    {
                        TempData["ErrorMessage"] = $"Sản phẩm {item.ProductVariant.Products?.ProductsName ?? "N/A"} không đủ hàng.";
                        model.User = currentUser;
                        model.CartItems = cartItems;
                        return View("Index", model);
                    }
                }

                // Cập nhật thông tin user
                bool userInfoChanged = false;

                if (currentUser.FullName != model.FullName)
                {
                    currentUser.FullName = model.FullName;
                    userInfoChanged = true;
                }

                if (currentUser.Phone != model.Phone)
                {
                    currentUser.Phone = model.Phone;
                    userInfoChanged = true;
                }

                if (currentUser.Email != model.Email)
                {
                    currentUser.Email = model.Email;
                    userInfoChanged = true;
                }

                if (userInfoChanged)
                {
                    db.Users.Update(currentUser);
                }

                if (currentUser.Address == null)
                {
                    currentUser.Address = new Address
                    {
                        Ward = model.Ward,
                        District = model.District,
                        City = model.City,
                        UserId = currentUser.UserId
                    };
                    db.Add(currentUser.Address);
                }
                else
                {
                    currentUser.Address.Ward = model.Ward;
                    currentUser.Address.District = model.District;
                    currentUser.Address.City = model.City;
                    db.Update(currentUser.Address);
                }

                db.SaveChanges();

                string fullAddress = $"{model.Ward}, {model.District}, {model.City}";

                var payment = new Payment
                {
                    PaymentMethod = PaymentMethod.CashOnDelivery,
                    IsActive = false,
                    CreatedAt = DateTime.Now
                };

                db.Payments.Add(payment);
                db.SaveChanges(); 

                // Tạo đơn hàng
                var order = new Orders
                {
                    UserId = currentUserId,
                    Address = fullAddress,  
                    Ward = model.Ward,
                    District = model.District,
                    City = model.City,
                    ShippingFee = model.ShippingFee,
                    TotalAmount = cartItems.Sum(i => i.Quantity * i.ProductVariant.Price) + model.ShippingFee,
                    OrderdAt = DateTime.Now,
                    OrderStatus = OrderStatus.Pending,
                    PaymentStatus = PaymentStatus.Pending,
                    PaymentId = payment.PaymentId, // liên kết với payment
                    OrderDetails = cartItems.Select(ci => new OrderDetail
                    {
                        ProductName = ci.ProductVariant?.Products?.ProductsName ?? "Sản phẩm không xác định",
                        Color = ci.ProductVariant?.Color ?? "",
                        Storage = ci.ProductVariant?.Storage ?? "",
                        Quantity = ci.Quantity,
                        UnitPrice = ci.ProductVariant?.Price ?? 0,
                        TotalPrice = ci.Quantity * (ci.ProductVariant?.Price ?? 0),
                        ProductVariantId = ci.ProductVariantId
                    }).ToList()
                };

                using var transaction = db.Database.BeginTransaction();

                db.Orders.Add(order);
                db.SaveChanges();


                // Giảm số lượng sản phẩm
                foreach (var item in cartItems)
                {
                    if (item.ProductVariant != null)
                    {
                        item.ProductVariant.Quantity -= item.Quantity;
                        if (item.ProductVariant.Quantity < 0)
                            item.ProductVariant.Quantity = 0;
                        db.ProductVariants.Update(item.ProductVariant);
                    }
                }
                db.SaveChanges();

                // Xóa giỏ hàng
                if (currentUser.Carts?.Items.Any() == true)
                {
                    db.CartItems.RemoveRange(currentUser.Carts.Items);
                    db.SaveChanges();
                }

                transaction.Commit();

                TempData["SuccessMessage"] = "Đặt hàng thành công!";
                return RedirectToAction("Success", new { orderId = order.OrdersId });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ LỖI KHI ĐẶT HÀNG:");
                Console.WriteLine(ex.ToString());

                TempData["ErrorMessage"] = "Có lỗi xảy ra khi đặt hàng: " + ex.Message;

                // Reload dữ liệu
                var userIdClaimCatch = User.FindFirst("UserId");
                if (userIdClaimCatch != null)
                {
                    var userIdCatch = long.Parse(userIdClaimCatch.Value);
                    var userCatch = db.Users
                        .Include(u => u.Address)
                        .Include(u => u.Carts)
                            .ThenInclude(c => c.Items)
                                .ThenInclude(ci => ci.ProductVariant)
                                    .ThenInclude(pv => pv.Products)
                                        .ThenInclude(p => p.ProductImages)
                        .FirstOrDefault(u => u.UserId == userIdCatch);

                    model.User = userCatch;
                    model.CartItems = userCatch?.Carts?.Items?.Where(ci => ci.ProductVariant != null)?.ToList() ?? new List<CartItem>();
                }

                return View("Index", model);
            }
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

        // Thêm action UpdateQuantity (từ phản hồi trước)
        [HttpPost("updatequantity")]
        public IActionResult UpdateQuantity(long cartItemId, int quantity)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null) return Json(new { success = false, message = "Chưa đăng nhập" });

                var userId = long.Parse(userIdClaim.Value);
                var cartItem = db.CartItems
                    .Include(ci => ci.ProductVariant)
                    .FirstOrDefault(ci => ci.CartItemId == cartItemId && ci.Carts.UserId == userId);

                if (cartItem == null) return Json(new { success = false, message = "Sản phẩm không tồn tại" });

                if (quantity < 1 || quantity > (cartItem.ProductVariant?.Quantity ?? 0))
                    return Json(new { success = false, message = "Số lượng không hợp lệ" });

                cartItem.Quantity = quantity;
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi cập nhật: " + ex.Message });
            }
        }
    }
}
