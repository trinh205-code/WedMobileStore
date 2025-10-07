namespace WebMobileStore.Models.Entity.Enums
{
    public enum PaymentStatus
    {
        Pending = 0,        // Chờ thanh toán
        Processing = 1,     // Đang xử lý
        Paid = 2,           // Đã thanh toán
        Failed = 3,         // Thanh toán thất bại
        Refunded = 4,       // Đã hoàn tiền
        Cancelled = 5,      // Đã hủy
        PartiallyPaid = 6,  // Thanh toán một phần
        Expired = 7         // Hết hạn
    }
}
