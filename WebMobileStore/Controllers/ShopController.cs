using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMobileStore.Models.Data;
using WebMobileStore.Models.Entity;

namespace WebMobileStore.Controllers
{
    [Route("Shop")]
    public class ShopController : Controller
    {
        private readonly MobileStoreContext db;

        public ShopController(MobileStoreContext context)
        {
            db = context;
        }

        // GET: /Shop
        [HttpGet("")]
        public IActionResult Index()
        {
            var products = db.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .Include(p => p.Brand)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            ViewBag.Title = "Tất cả sản phẩm";
            return View(products);
        }

        // GET: /Shop/Category/1
        [HttpGet("Category/{id}")]
        public IActionResult Category(long id)
        {
            var category = db.Categories.Find(id);
            if (category == null) return NotFound();

            var products = db.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .Include(p => p.Brand)
                .Where(p => p.IsActive && p.CategoryId == id)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            ViewBag.Title = category.CategoryName;
            ViewBag.CategoryName = category.CategoryName;
            return View("Index", products);
        }

        // GET: /Shop/ProductDetail/1
        [HttpGet("ProductDetail/{id}")]
        public IActionResult ProductDetail(long id)
        {
            var product = db.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .Include(p => p.Brand)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.Users)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null) return NotFound();

            ViewBag.Title = product.ProductsName;
            return View(product);
        }

        // GET: /Shop/Search?keyword=iphone
        [HttpGet("Search")]
        public IActionResult Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return RedirectToAction("Index");

            var products = db.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .Include(p => p.Brand)
                .Where(p => p.IsActive && p.ProductsName.Contains(keyword))
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            ViewBag.Title = $"Kết quả tìm kiếm: {keyword}";
            ViewBag.Keyword = keyword;
            return View("Index", products);
        }

        // GET: /Shop/NewArrivals
        [HttpGet("NewArrivals")]
        public IActionResult NewArrivals()
        {
            var products = db.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .Include(p => p.Brand)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Take(20)
                .ToList();

            ViewBag.Title = "Sản phẩm mới về";
            return View("Index", products);
        }

        // GET: /Shop/BestSellers
        [HttpGet("BestSellers")]
        public IActionResult BestSellers()
        {
            var products = db.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .Include(p => p.Brand)
                .Where(p => p.IsActive)
                .OrderBy(p => Guid.NewGuid())
                .Take(20)
                .ToList();

            ViewBag.Title = "Bán chạy nhất";
            return View("Index", products);
        }

        // GET: /Shop/Deals
        [HttpGet("Deals")]
        public IActionResult Deals()
        {
            var products = db.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .Include(p => p.Brand)
                .Where(p => p.IsActive && p.ProductVariants.Any(v => v.CompareAtPrice > v.Price))
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            ViewBag.Title = "Khuyến mãi hot";
            return View("Index", products);
        }
    }
}
