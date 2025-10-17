using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebMobileStore.Models.Data;
using WebMobileStore.Models.Entity;

namespace WebMobileStore.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly MobileStoreContext db;

        public AdminController(MobileStoreContext context)
        {
            db = context;
        }

        // Dashboard - Trang chủ Admin
        [HttpGet("")]
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            try
            {
                ViewBag.TotalUsers = db.Users?.Count() ?? 0;
                ViewBag.TotalProducts = db.Products?.Count() ?? 0;
                ViewBag.TotalOrders = db.Orders?.Count() ?? 0;
                ViewBag.TotalRevenue = db.Orders?.Sum(o => (decimal?)o.TotalAmount) ?? 0;

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
                var products = db.Products.ToList();

                return View(products);
            
        }

        [HttpGet("AddProductVariant/{id?}")]
        public IActionResult AddProductVariant(long? id)
        {
            try
            {
                var products = db.Products.ToList();

                ViewBag.Products = new SelectList(products, "ProductId", "ProductsName", id);

                var model = new ProductVariant
                {
                    ProductId = id ?? 0,
                    IsActive = true
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToAction("Products");
            }
        }



        [HttpPost("AddProductVariant")]
        [ValidateAntiForgeryToken]
        public IActionResult AddProductVariant(ProductVariant variant)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    variant.CreatedAt = DateTime.Now;
                    variant.IsActive = true;

                    db.ProductVariants.Add(variant);
                    db.SaveChanges();

                    TempData["Success"] = "Thêm biến thể sản phẩm thành công!";
                    return RedirectToAction("Products");
                }

                var products = db.Products.ToList();
                ViewBag.Products = new SelectList(products, "ProductId", "ProductsName", variant.ProductId);

                return View(variant);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                var products = db.Products.ToList();
                ViewBag.Products = new SelectList(products, "ProductId", "ProductsName", variant.ProductId);
                return View(variant);
            }
        }


        [HttpGet("AddProduct")]
        public IActionResult AddProduct()
        {
            try
            {
                ViewBag.Categories = db.Categories?.ToList() ?? new List<Categories>();
                ViewBag.Brands = db.Brands?.ToList() ?? new List<Brand>();

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
        public IActionResult AddProduct([Bind("ProductsName,CategoryId,BrandId,Price,Quantity,Description")] Products product)
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
                        ViewBag.Categories = db.Categories?.ToList() ?? new List<Categories>();
                        ViewBag.Brands = db.Brands?.ToList() ?? new List<Brand>();
                        return View(product);
                    }

                    if (product.Price <= 0)
                    {
                        TempData["Error"] = "Giá sản phẩm phải lớn hơn 0!";
                        ViewBag.Categories = db.Categories?.ToList() ?? new List<Categories>();
                        ViewBag.Brands = db.Brands?.ToList() ?? new List<Brand>();
                        return View(product);
                    }

                    // Kiểm tra category có tồn tại không
                    var categoryExists = db.Categories.Any(c => c.CategoryId == product.CategoryId);
                    if (!categoryExists)
                    {
                        TempData["Error"] = "Danh mục không tồn tại!";
                        ViewBag.Categories = db.Categories?.ToList() ?? new List<Categories>();
                        ViewBag.Brands = db.Brands?.ToList() ?? new List<Brand>();
                        return View(product);
                    }

                    db.Products.Add(product);
                    db.SaveChanges();

                    TempData["Success"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction("Products");
                }

                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["Error"] = "Có lỗi xảy ra: " + string.Join(", ", errors);
                ViewBag.Categories = db.Categories?.ToList() ?? new List<Categories>();
                ViewBag.Brands = db.Brands?.ToList() ?? new List<Brand>();
                return View(product);
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = "Lỗi cơ sở dữ liệu: " + (ex.InnerException?.Message ?? ex.Message);
                ViewBag.Categories = db.Categories?.ToList() ?? new List<Categories>();
                ViewBag.Brands = db.Brands?.ToList() ?? new List<Brand>();
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                ViewBag.Categories = db.Categories?.ToList() ?? new List<Categories>();
                ViewBag.Brands = db.Brands?.ToList() ?? new List<Brand>();
                return View(product);
            }
        }

        // EditProduct GET
        [HttpGet("EditProduct/{id}")]
        public IActionResult EditProduct(long id)
        {
            try
            {
                var product = db.Products.Find(id);

                if (product == null)
                {
                    TempData["Error"] = "Không tìm thấy sản phẩm!";
                    return RedirectToAction("Products");
                }

                ViewBag.Categories = db.Categories?.ToList() ?? new List<Categories>();
                ViewBag.Brands = db.Brands?.ToList() ?? new List<Brand>();

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
                    db.Update(product);
                    db.SaveChanges();

                    TempData["Success"] = "Cập nhật sản phẩm thành công!";
                    return RedirectToAction("Products");
                }

                ViewBag.Categories = db.Categories?.ToList() ?? new List<Categories>();
                ViewBag.Brands = db.Brands?.ToList() ?? new List<Brand>();
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                ViewBag.Categories = db.Categories?.ToList() ?? new List<Categories>();
                ViewBag.Brands = db.Brands?.ToList() ?? new List<Brand>();
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
                var product = db.Products.Find(id);

                if (product == null)
                {
                    TempData["Error"] = "Không tìm thấy sản phẩm!";
                    return RedirectToAction("Products");
                }

                // Kiểm tra sản phẩm có trong đơn hàng nào không
                var hasOrders = db.OrderDetails.Any(od => od.ProductVariant.ProductId == id);
                if (hasOrders)
                {
                    TempData["Error"] = "Không thể xóa sản phẩm đã có trong đơn hàng!";
                    return RedirectToAction("Products");
                }

                db.Products.Remove(product);
                db.SaveChanges();

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
                var orders = db.Orders
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
                var customers = db.Users
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
                var categories = db.Categories
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