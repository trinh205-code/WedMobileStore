using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMobileStore.Models.Data;
using WebMobileStore.ViewModels;

namespace WebMobileStore.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly MobileStoreContext _context;

        public HomeController(MobileStoreContext context)
        {
            _context = context;
        }

        [HttpGet("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return Redirect("Index");
        }


        [HttpGet("")] 
        [HttpGet("Index")] 
        public IActionResult Index()
        {
            try
            {
                var viewModel = new HomeViewModel
                {
                    Categories = _context.Categories
                        .Include(c => c.Brands)
                        .Take(8)
                        .ToList(),

                    FlashSaleProducts = _context.Products
                        .Include(p => p.ProductImages)
                        .Include(p => p.ProductVariants)
                        .Where(p => p.IsActive)
                        .OrderBy(p => Guid.NewGuid()) 
                        .Take(8)
                        .ToList(),

                    RecommendedProducts = _context.Products
                        .Include(p => p.ProductImages)
                        .Include(p => p.ProductVariants)
                        .Where(p => p.IsActive)
                        .OrderByDescending(p => p.CreatedAt)
                        .Take(12)
                        .ToList(),

                    NewProducts = _context.Products
                        .Include(p => p.ProductImages)
                        .Include(p => p.ProductVariants)
                        .Where(p => p.IsActive)
                        .OrderByDescending(p => p.CreatedAt)
                        .Take(8)
                        .ToList(),

                    TopSellingProducts = _context.Products
                        .Include(p => p.ProductImages)
                        .Include(p => p.ProductVariants)
                        .Where(p => p.IsActive)
                        .OrderBy(p => Guid.NewGuid())
                        .Take(8)
                        .ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải dữ liệu: " + ex.Message;
                return View(new HomeViewModel());
            }
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult Warranty()
        {
            return View();
        }

        public IActionResult Return()
        {
            return View();
        }

        public IActionResult Shipping()
        {
            return View();
        }
    }
}