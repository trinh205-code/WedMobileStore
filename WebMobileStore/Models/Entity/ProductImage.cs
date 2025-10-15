using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMobileStore.Models.Entity
{
    public class ProductImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ProductImageId { get; set; }
        public string ImageUrl { get; set; }
        public int DisplayOrder {  get; set; }
        public long ProductId { get; set; }
        public Products Products { get; set; }

    }
}
