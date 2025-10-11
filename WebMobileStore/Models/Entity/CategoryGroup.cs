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
        [Column(TypeName = "nvarchar(300)")]
        public string CategoryGroupName { get; set; }
        [Column(TypeName = "nvarchar(400)")]
        public string? Description { get; set; }
        public string? Image {  get; set; }

        public ICollection<Categories> Categories { get; set; }

    }
}
