using Microsoft.AspNetCore.Mvc;
using WebMobileStore.Models.Data;
using WebMobileStore.Models.Entity;
using Microsoft.EntityFrameworkCore;


namespace WebMobileStore.Controllers
{
    [Route("/Cart")]
    public class CartController : Controller
    {
        private readonly MobileStoreContext db;

        public CartController(MobileStoreContext context)
        {
            db = context;
        }


        [HttpGet("GetCart")]
        public IActionResult GetCart()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null)
                return Json(new { success = false, message = "User not logged in" });

            long userId = long.Parse(userIdClaim);

            var cart = db.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(v => v.Products)
                            .ThenInclude(p => p.ProductImages)
                .FirstOrDefault(c => c.UserId == userId);

            var cartItems = cart?.Items.Select(i => new
            {
                CartItemId = i.CartItemId, 
                ProductName = i.ProductVariant.Products.ProductsName,
                VariantName = $"{i.ProductVariant.Color} / {i.ProductVariant.Storage}",
                Quantity = i.Quantity,
                Price = i.ProductVariant.Price,
                ImageUrl = i.ProductVariant.Products.ProductImages.FirstOrDefault()?.ImageUrl
            }).ToList();


            return Json(new { success = true, cartItems });
        }

        [HttpPost("AddToCart")]
        public IActionResult AddToCart(long variantId, int quantity = 1)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null)
                return Json(new { success = false, message = "User not logged in" });

            long userId = long.Parse(userIdClaim);

            var cart = db.Carts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Carts { UserId = userId };
                db.Carts.Add(cart);
                db.SaveChanges();
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductVariantId == variantId);
            if (existingItem != null)
                existingItem.Quantity += quantity;
            else
                cart.Items.Add(new CartItem { ProductVariantId = variantId, Quantity = quantity });

            db.SaveChanges();

            int count = cart.Items.Sum(i => i.Quantity);
            return Json(new { success = true, cartCount = count });
        }

        [HttpPost("UpdateQuantity")]
        public IActionResult UpdateQuantity(long cartItemId, int quantity)
        {
            var userId = long.Parse(User.FindFirst("UserId").Value);
            var cartItem = db.CartItems.Include(ci => ci.Carts)
                .FirstOrDefault(ci => ci.CartItemId == cartItemId && ci.Carts.UserId == userId);

            if (cartItem == null) return Json(new { success = false, message = "Item not found" });

            cartItem.Quantity = quantity;
            db.SaveChanges();

            var count = db.CartItems.Where(ci => ci.Carts.UserId == userId).Sum(ci => ci.Quantity);

            return Json(new { success = true, cartCount = count });
        }

        [HttpPost("DeleteItem")]
        public IActionResult DeleteItem(long cartItemId)
        {
            var userId = long.Parse(User.FindFirst("UserId").Value);
            var item = db.CartItems.Include(ci => ci.Carts)
                .FirstOrDefault(ci => ci.CartItemId == cartItemId && ci.Carts.UserId == userId);

            if (item == null) return Json(new { success = false, message = "Item not found" });

            db.CartItems.Remove(item);
            db.SaveChanges();

            var count = db.CartItems.Where(ci => ci.Carts.UserId == userId).Sum(ci => ci.Quantity);

            return Json(new { success = true, cartCount = count, message = "Đã xoá" });
        }

        [HttpPost("Clear")]
        public IActionResult Clear()
        {
            var userId = long.Parse(User.FindFirst("UserId").Value);
            var items = db.CartItems.Include(ci => ci.Carts)
                .Where(ci => ci.Carts.UserId == userId);

            db.CartItems.RemoveRange(items);
            db.SaveChanges();

            return Json(new { success = true, cartCount = 0, message = "Đã xóa toàn bộ giỏ hàng" });
        }



    }
}

