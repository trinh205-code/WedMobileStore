using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using WebMobileStore.Models.Data;
using WebMobileStore.Models.Entity;

public static class DbInitializer
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new MobileStoreContext(
            serviceProvider.GetRequiredService<DbContextOptions<MobileStoreContext>>()))
        {
            // Tạo DB nếu chưa có
            context.Database.Migrate();

            // Nếu đã có dữ liệu thì không seed nữa
            if (context.Categories.Any())
            {
                return;
            }

            // === Seed CATEGORIES (Cha) ===
            var categories = new Categories[]
            {
                new Categories { CategoryName = "Điện thoại" },
                new Categories { CategoryName = "Laptop" },
                new Categories { CategoryName = "Tai nghe" },
                new Categories { CategoryName = "Camera" },
                new Categories { CategoryName = "Chuột" }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();

            var brands = new Brand[]
            {
                // Điện thoại
                new Brand { BrandName = "Apple",Image = "https://tse1.mm.bing.net/th/id/OIP.ULnBI0tcYY36IICea970DwHaJG?pid=Api&P=0&h=180" , CategoryId = categories[0].CategoryId },
                new Brand { BrandName = "Samsung",Image = "", CategoryId = categories[0].CategoryId },
                new Brand { BrandName = "Xiaomi",Image = "", CategoryId = categories[0].CategoryId },
                new Brand { BrandName = "Oppo",Image = "", CategoryId = categories[0].CategoryId },
                new Brand { BrandName = "Huawei",Image = "", CategoryId = categories[0].CategoryId },
                new Brand { BrandName = "Vivo",Image = "", CategoryId = categories[0].CategoryId },

                // Laptop
                new Brand { BrandName = "Dell",Image = "", CategoryId = categories[1].CategoryId },
                new Brand { BrandName = "MacBook",Image = "", CategoryId = categories[1].CategoryId },
                new Brand { BrandName = "HP",Image = "", CategoryId = categories[1].CategoryId },
                new Brand { BrandName = "Asus",Image = "", CategoryId = categories[1].CategoryId },

                // Tai nghe
                new Brand { BrandName = "Sony",Image = "", CategoryId = categories[2].CategoryId },
                new Brand { BrandName = "Samsung",Image = "", CategoryId = categories[2].CategoryId },
                new Brand { BrandName = "AirPods",Image = "", CategoryId = categories[2].CategoryId },

                // Camera
                new Brand { BrandName = "Ezviz",Image = "", CategoryId = categories[3].CategoryId },
                new Brand { BrandName = "TP-Link",Image = "", CategoryId = categories[3].CategoryId },
                new Brand { BrandName = "Imou",Image = "", CategoryId = categories[3].CategoryId },

                // Chuột
                new Brand { BrandName = "Logitech",Image = "", CategoryId = categories[4].CategoryId },
                new Brand { BrandName = "Genius",Image = "", CategoryId = categories[4].CategoryId },
                new Brand { BrandName = "Asus",Image = "", CategoryId = categories[4].CategoryId }
            };
            context.Brands.AddRange(brands);
            context.SaveChanges();
        }
    }
}
