using WebMobileStore.Models.Entity;

namespace WebMobileStore.ViewModels
{
    public class ShopViewModel
    {
        public List<Categories> Categories { get; set; }
        public List<Products> FlashSaleProducts { get; set; }
        public List<Products> RecommendedProducts { get; set; }
    }
}
