using WebMobileStore.Models.Entity.Enums;

namespace WebMobileStore.Models.Entity
{
    public class Payment
    {
        public long PaymentId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        Boolean IsActive { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        public long OrdersId { get; set; }
        public Orders Orders { get; set; }

    }
}
