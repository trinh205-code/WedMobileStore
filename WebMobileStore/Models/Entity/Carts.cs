namespace WebMobileStore.Models.Entity
{
    public class Carts
    {
        public long CartId {  get; set; }
        


        public long UserId { get; set; }
        public Users User { get; set; }

        public ICollection<CartItem> Items { get; set; }

    }
}
