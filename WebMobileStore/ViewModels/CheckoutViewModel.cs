using WebMobileStore.Models.Entity;

namespace WebMobileStore.ViewModels
{
    public class CheckoutViewModel
    {
        public Users User { get; set; }
        public List<CartItem> CartItems { get; set; }
        public double ShippingFee { get; set; }

        // Thông tin giao hàng
        public string Address { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }
    }
}
