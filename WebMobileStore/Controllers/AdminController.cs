using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebMobileStore.Models.Data;
using WebMobileStore.Models.Entity;
using WebMobileStore.Models.Entity.Enums;

namespace WebMobileStore.Controllers
{
    [Authorize(Roles = "Admin")]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Categories category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        // Products - Danh sách sản phẩm
        [HttpGet("Products")]
        public IActionResult Products(string searchTerm, long? categoryId, long? brandId)
        {
            var products = db.Products.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p => p.ProductsName.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            if (brandId.HasValue)
            {
                products = products.Where(p => p.BrandId == brandId.Value);
            }

            products = products
                       .Include(p => p.ProductImages) 
                       .Include(p => p.Brand)
                       .Include(p => p.Category);

            ViewBag.Categories = db.Categories.ToList();
            ViewBag.Brands = db.Brands.ToList();

            return View(products.ToList());
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

        [HttpPost("AddProduct")]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct([Bind("ProductsName,CategoryId,BrandId,Price,Quantity,Description")] Products product,
                                string ImageUrls)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Products.Add(product);
                    db.SaveChanges();

                    if (!string.IsNullOrWhiteSpace(ImageUrls))
                    {
                        var urls = ImageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        int order = 1;
                        foreach (var url in urls)
                        {
                            var trimmedUrl = url.Trim();
                            if (!string.IsNullOrEmpty(trimmedUrl))
                            {
                                var image = new ProductImage
                                {
                                    ProductId = product.ProductId,
                                    ImageUrl = trimmedUrl,
                                    DisplayOrder = order++
                                };
                                db.ProductImages.Add(image);
                            }
                        }

                        db.SaveChanges();
                    }

                    TempData["Success"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction("Products");
                }

                ViewBag.Categories = db.Categories.ToList();
                ViewBag.Brands = db.Brands.ToList();
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                ViewBag.Categories = db.Categories.ToList();
                ViewBag.Brands = db.Brands.ToList();
                return View(product);
            }
        }

        // EditProduct GET
        [HttpGet("EditProduct/{id}")]
        public IActionResult EditProduct(long id)
        {
            var product = db.Products
            .Include(p => p.ProductImages)
            .FirstOrDefault(p => p.ProductId == id);
            if (product == null) return NotFound();

            ViewBag.Categories = db.Categories
                                   .Select(c => new SelectListItem
                                   {
                                       Value = c.CategoryId.ToString(),
                                       Text = c.CategoryName
                                   }).ToList();

            ViewBag.Brands = db.Brands
                               .Select(b => new SelectListItem
                               {
                                   Value = b.BrandId.ToString(),
                                   Text = b.BrandName
                               }).ToList();

            return View(product);
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



        [HttpGet("AddProductVariant/{id?}")]
        public IActionResult AddProductVariant(long? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm!";
                return RedirectToAction("Products");
            }

            try
            {
                var product = db.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .FirstOrDefault(p => p.ProductId == id);

                if (product == null)
                {
                    TempData["Error"] = "Sản phẩm không tồn tại!";
                    return RedirectToAction("Products");
                }
                ViewBag.ProductId = product.ProductId;
                ViewBag.ProductName = product.ProductsName;
                ViewBag.CategoryName = product.Category?.CategoryName ?? "Không có";
                ViewBag.BrandName = product.Brand?.BrandName ?? "Không có";

                var model = new ProductVariant
                {
                    ProductId = product.ProductId,
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



        [HttpPost("AddProductVariant/{ProductId?}")]
        [ValidateAntiForgeryToken]
        public IActionResult AddProductVariant(long ProductId, [FromForm] List<ProductVariant> Variants)
        {
            if (Variants == null || Variants.Count == 0)
            {
                TempData["Error"] = "Chưa thêm biến thể nào!";
                return RedirectToAction("AddProductVariant", new { id = ProductId });
            }

            var existingVariants = db.ProductVariants
                .Where(v => v.ProductId == ProductId)
                .Select(v => new { v.Storage, v.Color })
                .ToList();

            foreach (var variant in Variants)
            {
                if (existingVariants.Any(v => v.Storage == variant.Storage && v.Color == variant.Color))
                {
                    TempData["Error"] = $"Biến thể {variant.Storage} - {variant.Color} đã tồn tại!";
                    return RedirectToAction("AddProductVariant", new { id = ProductId });
                }

                variant.ProductId = ProductId;
                variant.IsActive = true;
                variant.CreatedAt = DateTime.Now;
                db.ProductVariants.Add(variant);
            }

            db.SaveChanges();
            TempData["Success"] = "Thêm biến thể thành công!";
            return RedirectToAction("Variant", new { id = ProductId });
        }


        

        [HttpGet("Variant/{id}")]
        public IActionResult Variant(int id)
        {
            try
            {
                var product = db.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .FirstOrDefault(p => p.ProductId == id);

                if (product == null)
                {
                    TempData["Error"] = "Sản phẩm không tồn tại!";
                    return RedirectToAction("Products");
                }

                var variants = db.ProductVariants
                    .Where(v => v.ProductId == id)
                    .OrderBy(v => v.Storage)
                    .ThenBy(v => v.Color)
                    .ToList();

                ViewBag.ProductId = product.ProductId;
                ViewBag.ProductName = product.ProductsName;
                ViewBag.CategoryName = product.Category?.CategoryName ?? "Không có";
                ViewBag.BrandName = product.Brand?.BrandName ?? "Không có";

                return View(variants);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToAction("Products");
            }
        }



        [HttpPost("DeleteVariant/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteVariant(long id)
        {
            try
            {
                var variant = db.ProductVariants.Find(id);
                if (variant == null)
                {
                    TempData["Error"] = "Biến thể không tồn tại!";
                    return RedirectToAction("Variant");
                }

                var productId = variant.ProductId;

                var isUsedInOrders = db.OrderDetails.Any(od => od.ProductVariantId == id);

                if (isUsedInOrders)
                {
                    TempData["Error"] = "Không thể xóa biến thể này vì đã có trong đơn hàng!";
                    return RedirectToAction("Variant", new { id = productId });
                }

                var isUsedInCart = db.CartItems
                    .Any(ci => ci.ProductVariantId == id);

                if (isUsedInCart)
                {
                    var cartItems = db.CartItems.Where(ci => ci.ProductVariantId == id);
                    db.CartItems.RemoveRange(cartItems);
                }

                db.ProductVariants.Remove(variant);
                db.SaveChanges();

                TempData["Success"] = "Xóa biến thể thành công!";
                return RedirectToAction("Variant", new { id = productId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToAction("Products");
            }
        }




        [HttpGet("EditVariant/{id}")]
        public IActionResult EditVariant(long id)
        {
            try
            {
                var variant = db.ProductVariants
                    .Include(v => v.Products)
                    .ThenInclude(p => p.Category)
                    .Include(v => v.Products)
                    .ThenInclude(p => p.Brand)
                    .FirstOrDefault(v => v.ProductVariantId == id);

                if (variant == null)
                {
                    TempData["Error"] = "Biến thể không tồn tại!";
                    return RedirectToAction("Variant");
                }

                ViewBag.ProductId = variant.ProductId;
                ViewBag.ProductName = variant.Products.ProductsName;
                ViewBag.CategoryName = variant.Products.Category?.CategoryName ?? "Không có";
                ViewBag.BrandName = variant.Products.Brand?.BrandName ?? "Không có";

                return View(variant);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToAction("Variant");
            }
        }

        [HttpPost("EditVariant/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditVariant(long id, ProductVariant model)
        {
            try
            {
                var variant = db.ProductVariants.Find(id);
                if (variant == null)
                {
                    TempData["Error"] = "Biến thể không tồn tại!";
                    return RedirectToAction("Variant");
                }

                var isDuplicate = db.ProductVariants
                    .Any(v => v.ProductId == variant.ProductId
                           && v.Storage == model.Storage
                           && v.Color == model.Color
                           && v.ProductVariantId != id);

                if (isDuplicate)
                {
                    TempData["Error"] = $"Biến thể {model.Storage} - {model.Color} đã tồn tại!";
                    return RedirectToAction("EditVariant", new { id });
                }

                variant.Storage = model.Storage;
                variant.Color = model.Color;
                variant.Price = model.Price;
                variant.CompareAtPrice = model.CompareAtPrice;
                variant.Quantity = model.Quantity;
                variant.IsActive = model.IsActive;

                db.SaveChanges();
                TempData["Success"] = "Cập nhật biến thể thành công!";
                return RedirectToAction("Variant", new { id = variant.ProductId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToAction("Products");
            }
        }   



        [HttpPost("ActiveProduct/{id?}")]
        [ValidateAntiForgeryToken]
        public IActionResult ActiveProduct(long id)
        {
            var product = db.Products.Find(id);
            if (product == null) return View("Error");

            product.IsActive = !product.IsActive;
            db.Products.Update(product);
            db.SaveChanges();

            return RedirectToAction("Products");
        }


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

        [HttpGet("AddCategory")]
        public IActionResult AddCategory()
        {   

            return View(new Categories());
        }

        [HttpPost("AddCategory")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categories category)
        {
                db.Categories.Add(category);
                db.SaveChanges();
                TempData["Success"] = "Thêm danh mục thành công!";
                return RedirectToAction("Index"); 
            
        }
            
        [HttpPost("DeleteCategory/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(long id)
        {
            var category = db.Categories.FirstOrDefault(c => c.CategoryId == id);
            if(category == null)
            {
                return RedirectToAction("Category");
            }

            db.Categories.Remove(category);
            db.SaveChanges();

            return RedirectToAction("Categories");
        }


        [HttpGet("EditCategory/{id}")]
        public IActionResult EditCategory(int id)
        {
            var category = db.Categories.FirstOrDefault(b => b.CategoryId == id);
            if (category == null) return NotFound();

            ViewBag.Title = "Chỉnh sửa thương hiệu";
            ViewBag.Categories = db.Categories.ToList();

            return View("AddCategory", category);
        }


        [HttpPost("EditCategory/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(long id, Categories categories)
        {

            db.Categories.Update(categories);
            db.SaveChanges();
            return RedirectToAction("categories");

        }





        [HttpGet("Brands")]
        public IActionResult Brands()
        {
            var brands = db.Brands.Include(b => b.Category).ToList(); 
            return View(brands);
        }

        

        [HttpGet("AddBrand")]
        public IActionResult AddBrand()
        {
            ViewBag.Title = "Thêm thương hiệu";
            ViewBag.Categories = db.Categories.ToList();
            return View(new Brand()); 
        }

        
        [HttpPost("AddBrand")]
        public IActionResult AddBrand(Brand model)
        {
            ViewBag.Categories = db.Categories.ToList();
            db.Brands.Add(model);
            db.SaveChanges();
            return RedirectToAction("Brands");
        }


        [HttpGet("EditBrand/{id}")]
        public IActionResult EditBrand(int id)
        {
            var brand = db.Brands.FirstOrDefault(b => b.BrandId == id);
            if (brand == null) return NotFound();

            ViewBag.Title = "Chỉnh sửa thương hiệu";
            ViewBag.Categories = db.Categories.ToList();

            return View("AddBrand", brand);
        }


        [HttpPost("EditBrand/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditBrand(long id,Brand brand)
        {
            
                db.Brands.Update(brand);
                db.SaveChanges();
                return RedirectToAction("Brands");

        }

        [HttpPost("DeleteBrand/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteBrand(long id)
        {
            var brand = db.Brands.FirstOrDefault(b => b.BrandId == id);
            if (brand == null)
                return NotFound();

            db.Brands.Remove(brand);
            db.SaveChanges();

            return RedirectToAction("Brands");
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



        // GET: Sửa thông tin người dùng
        [HttpGet("EditUser/{id}")]
        public IActionResult EditUser(long id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Lưu thông tin chỉnh sửa
        [HttpPost("EditUser/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(long id, Users model)
        {
            if (id != model.UserId) return BadRequest();


            var user = db.Users.Find(id);
            if (user == null) return NotFound();


            user.FullName = model.FullName;
            user.Phone = model.Phone;
            user.role = model.role;

            db.SaveChanges();
            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Customers"); 
        }

        [HttpPost("SetAdmin/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult SetAdmin(long id)
        {
            var user = db.Users.Find(id);
            if (user == null) return NotFound();

            user.role = Role.Admin; 
            db.SaveChanges();

            TempData["Success"] = "Cấp quyền Admin thành công!";
            return RedirectToAction("Customers"); 
        }

       

        // Settings - Cài đặt
        [HttpGet("Settings")]
        public IActionResult Settings()
        {
            return View();
        }
    }
}