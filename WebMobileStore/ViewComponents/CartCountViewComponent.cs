using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMobileStore.Models.Data;

namespace WebMobileStore.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly MobileStoreContext db;
        private readonly IHttpContextAccessor context;

        public CartCountViewComponent(MobileStoreContext _db, IHttpContextAccessor _context)
        {
            db = _db;          
            context = _context; 
        }

        public IViewComponentResult Invoke()
        {
            var httpContext = context.HttpContext;
            if (httpContext == null)
                return View(0);

            var userIdClaim = httpContext.User?.FindFirst("UserId")?.Value;
            int count = 0;

            if (!string.IsNullOrEmpty(userIdClaim))
            {
                long userId = long.Parse(userIdClaim);
                count = db.CartItems
                           .Include(i => i.Carts)
                           .Where(i => i.Carts.UserId == userId)
                           .Sum(i => i.Quantity);
            }
            else
            {
                count = httpContext.Session?.GetInt32("CartCount") ?? 0;
            }

            return View(count);
        }
    }
}
