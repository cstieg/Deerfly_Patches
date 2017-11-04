using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Deerfly_Patches.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public decimal Shipping { get; set; }

        [Display(Name = "Display on Front Page")]
        public bool DisplayOnFrontPage { get; set; }

        [Display(Name = "Do Not Display")]
        public bool DoNotDisplay { get; set; }

        [StringLength(2000)]
        [AllowHtml]
        public string ProductInfo { get; set; }

        [Display(Name = "Product Images")]
        [InverseProperty("Product")]
        public virtual ICollection<WebImage> WebImages { get; set; }
    }
}
