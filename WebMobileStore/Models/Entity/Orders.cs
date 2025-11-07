using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebMobileStore.Models.Entity.Enums;

namespace WebMobileStore.Models.Entity
{
    public class Orders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long OrdersId { get; set; }
        public double TotalAmount { get; set; }
        public double DiscountAmount { get; set; }
        public double ShippingFee { get; set; }

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        [Column(TypeName = "nvarchar(300)")]
        public string Address { get; set; }
        [Column(TypeName = "nvarchar(300)")]
        public string Ward {  get; set; }
        [Column(TypeName = "nvarchar(300)")]
        public string District { get; set; }
        [Column(TypeName = "nvarchar(300)")]
        public string City { get; set; }

        public DateTime OrderdAt { get; set; } = DateTime.Now;


        public long UserId { get; set; }
        public Users User { get; set; }

        public long? PaymentId { get; set;}
        public Payment Payment { get; set; }    

        public ICollection<OrderDetail> OrderDetails { get; set; }

        public ICollection<Reviews> reviews { get; set; }

    }
}
