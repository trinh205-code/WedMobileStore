using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMobileStore.Models.Data;

namespace WebMobileStore.Controllers
{
    [Route("/cate")]
    public class TestController : Controller
    {
        private readonly MobileStoreContext db;

        public TestController(MobileStoreContext context)
        {
            db = context;
        }

        [Route("list")]
        public IActionResult Index()
        {
            // Lấy danh sách Category và include các Brand liên quan
            var categories = db.Categories
                .Include(c => c.Brands)
                .ToList();

            return View(categories);
        }
    }
}
