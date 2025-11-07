using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMobileStore.Models.Data;
using WebMobileStore.Models.Entity;
using WebMobileStore.ViewModels;

namespace WebMobileStore.Controllers
{
    [Route("/")]
    public class ShopController : Controller
    {
        private readonly MobileStoreContext db;

        public ShopController(MobileStoreContext context)
        {
            db = context;
        }



        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            // Lấy UserId nếu đã đăng nhập
            string userIdClaim = User.FindFirst("UserId")?.Value;
            long? userId = userIdClaim != null ? long.Parse(userIdClaim) : (long?)null;

            try
            {
                var viewModel = new HomeViewModel
                {
                    Categories = db.Categories
                        .Include(c => c.Brands)
                        .Take(8)
                        .ToList(),

                    FlashSaleProducts = db.Products
                        .Include(p => p.ProductImages)
                        .Include(p => p.ProductVariants)
                        .Where(p => p.IsActive)
                        .OrderBy(p => Guid.NewGuid())
                        .Take(8)
                        .ToList(),

                    RecommendedProducts = db.Products
                        .Include(p => p.ProductImages)
                        .Include(p => p.ProductVariants)
                        .Where(p => p.IsActive)
                        .OrderByDescending(p => p.CreatedAt)
                        .Take(12)
                        .ToList(),

                    NewProducts = db.Products
                        .Include(p => p.ProductImages)
                        .Include(p => p.ProductVariants)
                        .Where(p => p.IsActive)
                        .OrderByDescending(p => p.CreatedAt)
                        .Take(8)
                        .ToList(),

                    TopSellingProducts = db.Products
                        .Include(p => p.ProductImages)
                        .Include(p => p.ProductVariants)
                        .Where(p => p.IsActive)
                        .OrderBy(p => Guid.NewGuid())
                        .Take(8)
                        .ToList(),

                };

                if (userId != null)
                {
                    var cart = db.Carts
                                 .Include(c => c.Items)
                                 .FirstOrDefault(c => c.UserId == userId);

                    ViewBag.CartCount = cart?.Items?.Sum(ci => ci.Quantity) ?? 0;
                }
                else
                {
                    ViewBag.CartCount = 0;
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải dữ liệu: " + ex.Message;
                ViewBag.CartCount = 0;
                return View(new HomeViewModel());
            }
        }



        // GET: /Shop/Category/1
        [HttpGet("Category/{id}")]
        public IActionResult Category(long id)
        {
            var category = db.Categories.Find(id);
            if (category == null) return NotFound();

            var viewModel = new HomeViewModel
            {
                Categories = db.Categories.ToList(),

                FlashSaleProducts = db.Products
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductVariants)
                    .Where(p => p.IsActive)
                    .OrderBy(p => Guid.NewGuid())
                    .Take(8)
                    .ToList(),

                RecommendedProducts = db.Products
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductVariants)
                    .Where(p => p.IsActive)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(12)
                    .ToList(),

                NewProducts = db.Products
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductVariants)
                    .Where(p => p.IsActive)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(8)
                    .ToList(),

                TopSellingProducts = db.Products
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductVariants)
                    .Where(p => p.IsActive)
                    .OrderBy(p => Guid.NewGuid())
                    .Take(8)
                    .ToList(),

                TopBrand = db.Brands
                    .Where(b => b.CategoryId == 1)
                    .ToList() ?? new List<Brand>()
            };

            ViewBag.CategoryName = category.CategoryName;

            return View("Category", viewModel);
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
            return View("ProductSreach", products);
        }




        // GET: /Shop/ProductDetail/1
        [HttpGet("Shop/ProductDetail/{id}")]
        public IActionResult ProductDetail(long id)
        {
            var product = db.Products
                .Include(p => p.ProductVariants)
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpGet("Shop/GetAvailableColors")]
        public IActionResult GetAvailableColors(long productId, string storage)
        {
            var availableColors = db.ProductVariants
                .Where(v => v.ProductId == productId && v.Storage == storage)
                .Select(v => new { v.Color })  // ✅ chỉ lấy màu
                .Distinct()
                .ToList();

            return Json(availableColors);
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
