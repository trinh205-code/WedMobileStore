using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMobileStore.Models.Data;
using WebMobileStore.Models.Entity;
using WebMobileStore.Models.Entity.Enums;
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

            var categoryProducts = db.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .Where(p => p.CategoryId == id && p.IsActive)
                .ToList();

            var expensiveProducts = categoryProducts
                .OrderByDescending(p => p.Price)
                .Take(10)
                .ToList();

            var newProducts = categoryProducts
                .OrderByDescending(p => p.CreatedAt)
                .Take(10)
                .ToList();

            var topSellingProductIds = db.OrderDetails
                .Include(o => o.ProductVariant)
                .Where(o => o.ProductVariant.Products.CategoryId == id)
                .GroupBy(o => o.ProductVariant.Products.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(10)
                .ToList();

            var topSellingProducts = db.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .Where(p => topSellingProductIds.Select(x => x.ProductId).Contains(p.ProductId))
                .ToList();

            if (topSellingProducts.Count < 10)
            {
                var fillProducts = categoryProducts
                    .Where(p => !topSellingProducts.Select(x => x.ProductId).Contains(p.ProductId))
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(10 - topSellingProducts.Count)
                    .ToList();

                topSellingProducts.AddRange(fillProducts);
            }

            var viewModel = new HomeViewModel
            {
                Categories = db.Categories.ToList(),
                FlashSaleProducts = expensiveProducts,   
                RecommendedProducts = expensiveProducts, 
                NewProducts = newProducts,
                TopSellingProducts = topSellingProducts,
                TopBrand = db.Brands
                    .Where(b => b.CategoryId == id)
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



        [HttpGet("Shop/ProductDetail/{id}")]
        public IActionResult ProductDetail(long id)
        {
            var product = db.Products
            .Include(p => p.ProductVariants)
            .ThenInclude(v => v.OrderDetails) 
            .Include(p => p.ProductImages)
            .FirstOrDefault(p => p.ProductId == id);


            if (product == null)
                return NotFound();

            // 🔹 Tính tổng đánh giá
            int reviewCount = product.Reviews?.Count() ?? 0;

            // 🔹 Tính trung bình sao
            double avgRating = reviewCount > 0 ? product.Reviews.Average(r => r.Rating) : 5.0;

            int soldCount = product.ProductVariants?
            .SelectMany(v => v.OrderDetails ?? new List<OrderDetail>())  
            .Where(od => od.Orders != null && od.Orders.OrderStatus == OrderStatus.Completed)
            .Sum(od => od.Quantity) ?? 0;


            // 🔹 Kiểm tra hết hàng
            if (product.ProductVariants == null || !product.ProductVariants.Any(v => v.Quantity > 0))
            {
                ViewBag.OutOfStockMessage = "⚠️ Sản phẩm này hiện đã hết hàng.";
            }

            ViewBag.ReviewCount = reviewCount;
            ViewBag.AvgRating = avgRating;
            ViewBag.SoldCount = soldCount;

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
