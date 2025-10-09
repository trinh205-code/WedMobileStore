using Microsoft.EntityFrameworkCore;
using WebMobileStore.Models.Data;

var builder = WebApplication.CreateBuilder(args);

//Đằng ký SchoolContext là một DbContext của ứng dung
builder.Services.AddDbContext<MobileStoreContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("MobileStoreContext")));



// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    DbInitializer.Initialize(services);
//}



    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
