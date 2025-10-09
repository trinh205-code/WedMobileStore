using Microsoft.EntityFrameworkCore;

namespace WebMobileStore.Models.Data
{
    public class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MobileStoreContext(serviceProvider
            .GetRequiredService<DbContextOptions<MobileStoreContext>>()))
            {
                context.Database.EnsureCreated();
                context.SaveChanges();
                return;
            }

        }
    }
}
