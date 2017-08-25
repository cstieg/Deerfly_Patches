using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Deerfly_Patches.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public decimal Shipping { get; set; }

        [StringLength(255)]
        [DisplayName("Upload image file")]
        public string ImageURL { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        public bool DisplayOnFrontPage { get; set; }

        public string PayPalUrl { get; set; }
    }
}
