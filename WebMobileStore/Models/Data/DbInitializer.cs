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
                new Brand { BrandName = "Apple",Image = "https://icons8.com/icon/30840/apple-inc" , CategoryId = categories[0].CategoryId },
                new Brand { BrandName = "Samsung",Image = "https://www.flaticon.com/free-icon/samsung_882849", CategoryId = categories[0].CategoryId },
                new Brand { BrandName = "Xiaomi",Image = "https://www.flaticon.com/free-icon/xiaomi_882720", CategoryId = categories[0].CategoryId },
                new Brand { BrandName = "Oppo",Image = "https://cdn-icons-png.flaticon.com/128/882/882745.png", CategoryId = categories[0].CategoryId },
                new Brand { BrandName = "Huawei",Image = "https://img.icons8.com/?size=160&id=oOaQa2nsHmDV&format=png", CategoryId = categories[0].CategoryId },
                new Brand { BrandName = "Vivo",Image = "https://cdn-icons-png.flaticon.com/128/882/882711.png", CategoryId = categories[0].CategoryId },

                // Laptop
                new Brand { BrandName = "Dell",Image = "https://cdn-icons-png.flaticon.com/128/882/882828.png", CategoryId = categories[1].CategoryId },
                new Brand { BrandName = "MacBook",Image = "https://icons8.com/icon/30840/apple-inc", CategoryId = categories[1].CategoryId },
                new Brand { BrandName = "HP",Image = "https://cdn-icons-png.flaticon.com/128/16183/16183582.png", CategoryId = categories[1].CategoryId },
                new Brand { BrandName = "Asus",Image = "https://cdn-icons-png.flaticon.com/128/5969/5969002.png", CategoryId = categories[1].CategoryId },

                // Tai nghe
                new Brand { BrandName = "Sony",Image = "https://www.flaticon.com/free-icon/sony_5969287", CategoryId = categories[2].CategoryId },
                new Brand { BrandName = "Samsung",Image = "https://www.flaticon.com/free-icon/sony_5969287", CategoryId = categories[2].CategoryId },
                new Brand { BrandName = "AirPods",Image = "https://icons8.com/icon/30840/apple-inc", CategoryId = categories[2].CategoryId },

                // Camera
                new Brand { BrandName = "Ezviz",Image = "https://vectorseek.com/wp-content/uploads/2023/09/Ezviz-Logo-Vector.svg-.png", CategoryId = categories[3].CategoryId },
                new Brand { BrandName = "TP-Link",Image = "https://seeklogo.com/vector-logo/291223/tp-link-nuevo", CategoryId = categories[3].CategoryId },
                new Brand { BrandName = "Imou",Image = "https://www.svgrepo.com/show/330690/imou.svg", CategoryId = categories[3].CategoryId },

                // Chuột
                new Brand { BrandName = "Logitech",Image = "https://img.icons8.com/?size=100&id=9zVjmNkFCnhC&format=png", CategoryId = categories[4].CategoryId },
                new Brand { BrandName = "Genius",Image = "https://img.icons8.com/?size=100&id=xhd-ctueaF5j&format=png", CategoryId = categories[4].CategoryId },
                new Brand { BrandName = "Asus",Image = "https://www.flaticon.com/free-icon/asus_5969050", CategoryId = categories[4].CategoryId }
            };
            context.Brands.AddRange(brands);
            context.SaveChanges();
        }
    }
}
