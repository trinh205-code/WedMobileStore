using WebMobileStore.Models.Entity;

namespace WebMobileStore.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Categories> Categories { get; set; }
        public IEnumerable<Products> FlashSaleProducts { get; set; }
        public IEnumerable<Products> RecommendedProducts { get; set; }
        public IEnumerable<Products> NewProducts { get; set; }
        public IEnumerable<Products> TopSellingProducts { get; set; }

        public IEnumerable<Brand> TopBrand { get; set; }
    }
}