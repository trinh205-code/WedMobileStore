using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMobileStore.Models.Entity
{
    public class Address
    {
        [Key]  
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AddressId { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string Ward {  get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string District { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string City { get; set; }

        public long UserId { get; set; }
        public Users Users { get; set; }

    }
}
