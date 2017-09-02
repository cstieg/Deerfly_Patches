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

        [DisplayName("Upload image file")]
        public string ImageUrl { get; set; }
        public string ImageSrcSet { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        public bool DisplayOnFrontPage { get; set; }

        [Url(ErrorMessage = "Enter valid Url beginning with 'http://' or 'https://'")]
        [DisplayName("PayPal purchase URL")]
        public string PayPalUrl { get; set; }
    }
}
