using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMobileStore.Models.Entity
{
    public class Brand
    {

        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BrandId { get; set; }
        [Column(TypeName = "nvarchar(300)")]
        public string BrandName { get; set; }
        [Column(TypeName = "nvarchar(400)")]
        public string? Description { get; set; }
        public string? Image {  get; set; }

        public long CategoryId { get; set; }
        public Categories Category { get; set; }

        public ICollection<Products> Products { get; set; }
    }
}
