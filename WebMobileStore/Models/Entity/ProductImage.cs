namespace WebMobileStore.Models.Entity
{
    public class ProductImage
    {
        public long ProductImageId { get; set; }
        public string ImageUrl { get; set; }
        public int DisplayOrder {  get; set; }



        public long ProductId { get; set; }
        public Products Products { get; set; }

    }
}
