using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMobileStore.Models.Data;
using WebMobileStore.Models.Entity;

namespace WebMobileStore.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly MobileStoreContext _context;

        public AdminController(MobileStoreContext context)
        {
            _context = context;
        }

        // Dashboard - Trang chủ Admin
        [HttpGet("")]
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            try
            {
                ViewBag.TotalUsers = _context.Users?.Count() ?? 0;
                ViewBag.TotalProducts = _context.Products?.Count() ?? 0;
                ViewBag.TotalOrders = _context.Orders?.Count() ?? 0;
                ViewBag.TotalRevenue = _context.Orders?.Sum(o => (decimal?)o.TotalAmount) ?? 0;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải dữ liệu dashboard: " + ex.Message;
                ViewBag.TotalUsers = 0;
                ViewBag.TotalProducts = 0;
                ViewBag.TotalOrders = 0;
                ViewBag.TotalRevenue = 0;

                return View();
            }
        }

        // Products - Danh sách sản phẩm
        [HttpGet("Products")]
        public IActionResult Products()
        {
            try
            {
                var products = _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductVariants)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToList();

                return View(products ?? new List<Products>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải danh sách sản phẩm: " + ex.Message;
                return View(new List<Products>());
            }
        }

        // AddProduct GET - Form thêm sản phẩm
        [HttpGet("AddProduct")]
        public IActionResult AddProduct()
        {
            try
            {
                ViewBag.Categories = _context.Categories?.ToList() ?? new List<Categories>();
                ViewBag.Brands = _context.Brands?.ToList() ?? new List<Brand>();

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải form thêm sản phẩm: " + ex.Message;
                ViewBag.Categories = new List<Categories>();
                ViewBag.Brands = new List<Brand>();

                return View();
            }
        }

        // AddProduct POST - Xử lý thêm sản phẩm
        [HttpPost("AddProduct")]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct(Products product)
        {
            try
            {
                if (product == null)
                {
                    TempData["Error"] = "Dữ liệu sản phẩm không hợp lệ!";
                    return RedirectToAction("AddProduct");
                }

                if (ModelState.IsValid)
                {
                    // Kiểm tra các trường bắt buộc
                    if (string.IsNullOrWhiteSpace(product.ProductsName))
                    {
                        TempData["Error"] = "Tên sản phẩm không được để trống!";
                        ViewBag.Categories = _context.Categories?.ToList() ?? new List<Categories>();
                        ViewBag.Brands = _context.Brands?.ToList() ?? new List<Brand>();
                        return View(product);
                    }

                    if (product.Price <= 0)
                    {
                        TempData["Error"] = "Giá sản phẩm phải lớn hơn 0!";
                        ViewBag.Categories = _context.Categories?.ToList() ?? new List<Categories>();
                        ViewBag.Brands = _context.Brands?.ToList() ?? new List<Brand>();
                        return View(product);
                    }

                    // Kiểm tra category có tồn tại không
                    var categoryExists = _context.Categories.Any(c => c.CategoryId == product.BrandId);
                    if (!categoryExists)
                    {
                        TempData["Error"] = "Danh mục không tồn tại!";
                        ViewBag.Categories = _context.Categories?.ToList() ?? new List<Categories>();
                        ViewBag.Brands = _context.Brands?.ToList() ?? new List<Brand>();
                        return View(product);
                    }

                    _context.Products.Add(product);
                    _context.SaveChanges();

                    TempData["Success"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction("Products");
                }

                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["Error"] = "Có lỗi xảy ra: " + string.Join(", ", errors);
                ViewBag.Categories = _context.Categories?.ToList() ?? new List<Categories>();
                ViewBag.Brands = _context.Brands?.ToList() ?? new List<Brand>();
                return View(product);
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = "Lỗi cơ sở dữ liệu: " + (ex.InnerException?.Message ?? ex.Message);
                ViewBag.Categories = _context.Categories?.ToList() ?? new List<Categories>();
                ViewBag.Brands = _context.Brands?.ToList() ?? new List<Brand>();
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                ViewBag.Categories = _context.Categories?.ToList() ?? new List<Categories>();
                ViewBag.Brands = _context.Brands?.ToList() ?? new List<Brand>();
                return View(product);
            }
        }

        // EditProduct GET
        [HttpGet("EditProduct/{id}")]
        public IActionResult EditProduct(long id)
        {
            try
            {
                var product = _context.Products.Find(id);

                if (product == null)
                {
                    TempData["Error"] = "Không tìm thấy sản phẩm!";
                    return RedirectToAction("Products");
                }

                ViewBag.Categories = _context.Categories?.ToList() ?? new List<Categories>();
                ViewBag.Brands = _context.Brands?.ToList() ?? new List<Brand>();

                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToAction("Products");
            }
        }

        // EditProduct POST
        [HttpPost("EditProduct/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(long id, Products product)
        {
            try
            {
                if (id != product.ProductId)
                {
                    TempData["Error"] = "Dữ liệu không hợp lệ!";
                    return RedirectToAction("Products");
                }

                if (ModelState.IsValid)
                {
                    _context.Update(product);
                    _context.SaveChanges();

                    TempData["Success"] = "Cập nhật sản phẩm thành công!";
                    return RedirectToAction("Products");
                }

                ViewBag.Categories = _context.Categories?.ToList() ?? new List<Categories>();
                ViewBag.Brands = _context.Brands?.ToList() ?? new List<Brand>();
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                ViewBag.Categories = _context.Categories?.ToList() ?? new List<Categories>();
                ViewBag.Brands = _context.Brands?.ToList() ?? new List<Brand>();
                return View(product);
            }
        }

        // DeleteProduct
        [HttpPost("DeleteProduct/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProduct(long id)
        {
            try
            {
                var product = _context.Products.Find(id);

                if (product == null)
                {
                    TempData["Error"] = "Không tìm thấy sản phẩm!";
                    return RedirectToAction("Products");
                }

                // Kiểm tra sản phẩm có trong đơn hàng nào không
                var hasOrders = _context.OrderDetails.Any(od => od.ProductVariant.ProductId == id);
                if (hasOrders)
                {
                    TempData["Error"] = "Không thể xóa sản phẩm đã có trong đơn hàng!";
                    return RedirectToAction("Products");
                }

                _context.Products.Remove(product);
                _context.SaveChanges();

                TempData["Success"] = "Xóa sản phẩm thành công!";
                return RedirectToAction("Products");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToAction("Products");
            }
        }

        // Orders - Danh sách đơn hàng
        [HttpGet("Orders")]
        public IActionResult Orders()
        {
            try
            {
                var orders = _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductVariant)
                    .OrderByDescending(o => o.OrderdAt)
                    .ToList();

                return View(orders ?? new List<Orders>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải danh sách đơn hàng: " + ex.Message;
                return View(new List<Orders>());
            }
        }

        // Customers - Danh sách khách hàng
        [HttpGet("Customers")]
        public IActionResult Customers()
        {
            try
            {
                var customers = _context.Users
                    .Include(u => u.Address)
                    .Include(u => u.Orders)
                    .OrderByDescending(u => u.CreatedAt)
                    .ToList();

                return View(customers ?? new List<Users>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải danh sách khách hàng: " + ex.Message;
                return View(new List<Users>());
            }
        }

        // Categories - Danh sách danh mục
        [HttpGet("Categories")]
        public IActionResult Categories()
        {
            try
            {
                var categories = _context.Categories
                    .Include(c => c.Brands)
                    .ToList();

                return View(categories ?? new List<Categories>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải danh sách danh mục: " + ex.Message;
                return View(new List<Categories>());
            }
        }

        // Settings - Cài đặt
        [HttpGet("Settings")]
        public IActionResult Settings()
        {
            return View();
        }
    }
}