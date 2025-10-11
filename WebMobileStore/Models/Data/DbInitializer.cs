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

            // Nếu bảng CategoryGroup đã có dữ liệu thì không seed lại
            if (context.CategoryGroups.Any())
            {
                return;
            }

            // === Seed CategoryGroup ===
            var categoryGroups = new CategoryGroup[]
            {
                new CategoryGroup { CategoryGroupName = "Điện thoại" },
                new CategoryGroup { CategoryGroupName = "Laptop" },
                new CategoryGroup { CategoryGroupName = "Tai nghe" },
                new CategoryGroup { CategoryGroupName = "Camera" },
                new CategoryGroup { CategoryGroupName = "Chuột" }
            };
            context.CategoryGroups.AddRange(categoryGroups);
            context.SaveChanges();

            var categories = new Categories[]
            {
                // Điện thoại
                new Categories { CategoryName = "Apple", CategoryGroupId = categoryGroups[0].CategoryGroupId },
                new Categories { CategoryName = "Samsung", CategoryGroupId = categoryGroups[0].CategoryGroupId },
                new Categories { CategoryName = "Xiaomi", CategoryGroupId = categoryGroups[0].CategoryGroupId },
                new Categories { CategoryName = "Oppo", CategoryGroupId = categoryGroups[0].CategoryGroupId },
                new Categories { CategoryName = "Huawei", CategoryGroupId = categoryGroups[0].CategoryGroupId },
                new Categories { CategoryName = "Vivo", CategoryGroupId = categoryGroups[0].CategoryGroupId },

                // Laptop
                new Categories { CategoryName = "Dell", CategoryGroupId = categoryGroups[1].CategoryGroupId },
                new Categories { CategoryName = "MacBook", CategoryGroupId = categoryGroups[1].CategoryGroupId },
                new Categories { CategoryName = "HP", CategoryGroupId = categoryGroups[1].CategoryGroupId },
                new Categories { CategoryName = "Asus", CategoryGroupId = categoryGroups[1].CategoryGroupId },

                // Tai nghe
                new Categories { CategoryName = "Sony", CategoryGroupId = categoryGroups[2].CategoryGroupId },
                new Categories { CategoryName = "Samsung", CategoryGroupId = categoryGroups[2].CategoryGroupId },
                new Categories { CategoryName = "AirPods", CategoryGroupId = categoryGroups[2].CategoryGroupId },

                // Camera
                new Categories { CategoryName = "Ezviz", CategoryGroupId = categoryGroups[3].CategoryGroupId },
                new Categories { CategoryName = "TP-Link", CategoryGroupId = categoryGroups[3].CategoryGroupId },
                new Categories { CategoryName = "Imou", CategoryGroupId = categoryGroups[3].CategoryGroupId },

                // Chuột
                new Categories { CategoryName = "Logitech", CategoryGroupId = categoryGroups[4].CategoryGroupId },
                new Categories { CategoryName = "Genius", CategoryGroupId = categoryGroups[4].CategoryGroupId },
                new Categories { CategoryName = "Asus", CategoryGroupId = categoryGroups[4].CategoryGroupId }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();
        }
    }
}
