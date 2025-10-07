namespace WebMobileStore.Models.Entity.Enums
{
    public enum OrderStatus
    {
        Pending = 0,      // Đơn hàng mới
        Confirmed = 1,    // Đã xác nhận
        Processing = 2,   // Đang xử lý
        Shipped = 3,      // Đã gửi hàng
        Delivered = 4,    // Đã giao thành công
        Cancelled = 5,    // Đã hủy
        Returned = 6,     // Bị trả lại
        Completed = 7     // Hoàn tất
    }
}
