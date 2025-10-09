using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebMobileStore.Models.Entity.Enums;

namespace WebMobileStore.Models.Entity
{
    public class Users
    {
        public Users() {
            Carts = new Carts();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserId { get; set; }
        public string FullName {  get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone {  get; set; }
        public Role role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public long CartId { get; set; }
        public Carts Carts { get; set; }

        public ICollection<Orders> Orders { get; set; }
        public long AddressId { get; set; }
        public Address Address { get; set; }

        public ICollection<Reviews> Reviews { get; set; }   

        
    }
}
