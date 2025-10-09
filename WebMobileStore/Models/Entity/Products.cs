using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMobileStore.Models.Entity
{
    public class Products
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ProductId { get; set; }
        [Column(TypeName = "nvarchar(300)")]
        public string ProductsName { get; set; }

        [Column(TypeName = "nvarchar(300)")]
        public string? Description { get; set; }

        public double Price { get; set; }
        public int Quantity { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public long CategoriesId { get; set; }
        public Categories Categories { get; set; }

        public ICollection<CartItem> CartItems { get; set; }

        public ICollection<ProductImage> ProductImages { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<Reviews> Reviews { get; set; }
    }
}
