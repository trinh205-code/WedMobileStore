namespace WebMobileStore.Models.Entity
{
    public class Address
    {
        public long AddressId { get; set; }
        public string Ward {  get; set; }
        public string District { get; set; }
        public string City { get; set; }

        public long UserId { get; set; }
        public Users Users { get; set; }

    }
}
