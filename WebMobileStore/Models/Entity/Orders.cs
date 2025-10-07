using WebMobileStore.Models.Entity.Enums;

namespace WebMobileStore.Models.Entity
{
    public class Orders
    {
        public long OrdersId { get; set; }
        public double TotalAmount { get; set; }
        public double DiscountAmount { get; set; }
        public double ShippingFee { get; set; }

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        public string Address { get; set; }
        public string Ward {  get; set; }
        public string District { get; set; }
        public string City { get; set; }

        public DateTime OrderdAt { get; set; } = DateTime.Now;


        public long UserId { get; set; }
        public Users User { get; set; }

        public long PaymentId { get; set;}
        public Payment Payment { get; set; }    

        public ICollection<OrderDetail> OrderDetails { get; set; }

    }
}
