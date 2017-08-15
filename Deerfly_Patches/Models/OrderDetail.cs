using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deerfly_Patches.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }

        [ForeignKey("Product")]
        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        public DateTime PlacedInCart { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public decimal Shipping { get; set; }

        public bool CheckedOut { get; set; }

        [ForeignKey("Order")]
        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [ReadOnly(true)]
        public decimal ExtendedPrice
        {
            get
            {
                return Quantity * UnitPrice;
            }

        }

        [ReadOnly(true)]
        public decimal TotalPrice
        {
            get 
            {
                return ExtendedPrice + Shipping;
            }
        }

        public override string ToString()
        {
            return Product.ToString() + " Qty " + Quantity.ToString();
        }
    }
}
