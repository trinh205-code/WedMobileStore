using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebMobileStore.Models.Entity.Enums;

namespace WebMobileStore.Models.Entity
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PaymentId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        // k chuaw id order de tranh vong lap

        public long? OrderId { get; set; }
        public Orders? Orders { get; set; }

    }
}
