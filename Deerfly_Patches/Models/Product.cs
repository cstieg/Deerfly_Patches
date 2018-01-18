using Cstieg.Sales.Models;
using System.ComponentModel.DataAnnotations;

namespace DeerflyPatches.Models
{
    public class Product : ProductBase
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public override string Name { get; set; }
    }
}

