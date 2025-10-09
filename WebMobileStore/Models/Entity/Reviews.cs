using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMobileStore.Models.Entity
{
    public class Reviews
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ReviewsId { get; set; }
        public int Rating { get; set; }

        [Column(TypeName = "nvarchar(300)")]
        public string? ReviewsTitle { get; set; }
        [Column(TypeName = "nvarchar(300)")]
        public string? ReviewsCotent {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public long UserId { get; set; }    
        public Users Users { get; set; }

        public long ProductId { get; set; } 
        public Products Products { get; set; }

        public long OrderId { get; set; }
        public Orders Orders { get; set; }
    }
}
