using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMobileStore.Models.Entity
{
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CartItemId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public long CartId { get; set; }
        public Carts Carts { get; set; }


        public long ProductsId { get; set; }
        public Products Products { get; set; }

    }
}
