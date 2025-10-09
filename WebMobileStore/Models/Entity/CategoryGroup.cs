using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMobileStore.Models.Entity
{
    public class CategoryGroup
    {

        public CategoryGroup() {
            Categories = new List<Categories>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CategoryGroupId { get; set; }
        public string CategoryGroupName { get; set; }
        public string Description { get; set; }
        public String Image {  get; set; }

        public ICollection<Categories> Categories { get; set; }

    }
}
