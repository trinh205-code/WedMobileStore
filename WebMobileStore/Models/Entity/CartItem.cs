namespace WebMobileStore.Models.Entity
{
    public class CartItem
    {
        public long CartItemId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public long CartsId { get; set; }
        public Carts Carts { get; set; }


        public long ProductsId { get; set; }
        public Products Products { get; set; }

    }
}
