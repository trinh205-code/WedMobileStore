using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMobileStore.Models.Entity
{
        public class Categories
        {

            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public long CategoryId { get; set; }
            public string CategoryName { get; set; }
            public long CategoryGroupId { get; set; }

            public CategoryGroup CategoryGroup { get; set; }

            public ICollection<Products> Products { get; set; }


        }
}
