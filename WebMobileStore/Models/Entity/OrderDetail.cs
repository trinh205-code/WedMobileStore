using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMobileStore.Models.Entity
{
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long OrderDetailId { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string ProductName { get; set; }
        public string Color { get; set; } 
        public string Storage { get; set; } 
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }

        public long OrderId { get; set; }
        public Orders Orders { get; set; }

        public long ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }



    }
}
