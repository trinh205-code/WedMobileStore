using System.ComponentModel.DataAnnotations;
using WebMobileStore.Models.Entity;

namespace WebMobileStore.ViewModels
{
    public class CheckoutViewModel
    {
        public Users User { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Vui lòng nhập phường/xã")]
        public string Ward { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập quận/huyện")]
        public string District { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tỉnh/thành phố")]
        public string City { get; set; }

        public string PaymentMethod { get; set; } = "COD";

        public double ShippingFee { get; set; } = 30000;
    }

}
