using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMobileStore.Models.Entity
{
    public class Carts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CartId {  get; set; }
        


        public long UserId { get; set; }
        public Users User { get; set; }

        public ICollection<CartItem> Items { get; set; }

    }
}
