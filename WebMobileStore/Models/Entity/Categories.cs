namespace WebMobileStore.Models.Entity
{
    public class Categories
    {

        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public long CategoryGroupId { get; set; }

        public CategoryGroup CategoryGroup { get; set; }

        public ICollection<Products> Products { get; set; }


    }
}
