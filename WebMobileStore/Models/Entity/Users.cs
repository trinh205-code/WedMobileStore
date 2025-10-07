using WebMobileStore.Models.Entity.Enums;

namespace WebMobileStore.Models.Entity
{
    public class Users
    {

        public Users() {
            Carts = new Carts();
        }
        public long UserId { get; set; }
        string FullName {  get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string Phone {  get; set; }
        Role role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Carts Carts { get; set; }

        public ICollection<Orders> Orders { get; set; }

        public long AddressId { get; set; }
        public Address Address { get; set; }
    }
}
