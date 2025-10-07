namespace WebMobileStore.Models.Entity
{
    public class Products
    {
        public long ProductsId { get; set; }
        public string ProductsName { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }
        public int Quantity { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public long CategoriesId { get; set; }
        public Categories Categories { get; set; }

        public ICollection<CartItem> CartItems { get; set; }

        public ICollection<ProductImage> ProductImages { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
