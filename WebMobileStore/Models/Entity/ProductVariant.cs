using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMobileStore.Models.Entity
{
    public class ProductVariant
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ProductVariantId { get; set; }
        [Column(TypeName = "nvarchar(300)")]
        public string Color { get; set; } 
        public string Storage { get; set; } // "128GB", "256GB", "512GB"
        public double Price { get; set; }
        public double CompareAtPrice { get; set; } // Giá gốc (để hiển thị giảm giá)
        public int Quantity { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

     
        public long ProductId { get; set; }
        public Products Products { get; set; }

        
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
