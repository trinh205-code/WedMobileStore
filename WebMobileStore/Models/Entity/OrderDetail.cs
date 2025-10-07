namespace WebMobileStore.Models.Entity
{
    public class OrderDetail
    {
        public long OrderDetailId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set;}

        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }

        public long OrderId { get; set; }
        public Orders Orders { get; set; }


        public long ProductId { get; set; }
        public Products Products { get; set; }



    }
}
