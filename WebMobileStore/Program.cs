using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebMobileStore.Models.Data;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký DbContext
builder.Services.AddDbContext<MobileStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MobileStoreContext")));

// Add services to the container
builder.Services.AddControllersWithViews();

// Cấu hình Authentication Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";   // Khi chưa login
        options.LogoutPath = "/User/Logout"; // Khi logout
        options.AccessDeniedPath = "/AccessDenied"; // (tuỳ chọn)
    });

// ---- PHẢI tạo app sau khi cấu hình service ----
var app = builder.Build();

// Khởi tạo database nếu cần
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    DbInitializer.Initialize(services);
}

// Cấu hình pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

// 🟢 Thứ tự rất quan trọng:
app.UseAuthentication(); // Luôn trước Authorization
app.UseAuthorization();

// Cấu hình route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
