using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deerfly_Patches.Models
{
    public class Order
    {
        public Order()
        {
            OrderDetails = new List<OrderDetail>();
        }

        [Key]
        public int OrderId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [Required]
        public DateTime DateOrdered { get; set; }

        [ForeignKey("ShipToAddress")]
        public int ShipToAddressId { get; set; }
        public virtual Address ShipToAddress { get; set; }

        [ForeignKey("BillToAddress")]
        public int BillToAddressId { get; set; }
        public virtual Address BillToAddress { get; set; }

        [InverseProperty("Order")]
        public virtual List<OrderDetail> OrderDetails { get; set; }

        public decimal Subtotal {
            get
            {
                decimal subtotal = 0;
                for (int i = 0; i < OrderDetails.Count; i++)
                {
                    subtotal += OrderDetails[i].ExtendedPrice;
                }
                return subtotal;
            }
        }
        
        public decimal Shipping {
            get
            {
                decimal shipping = 0;
                for (int i = 0; i < OrderDetails.Count; i++)
                {
                    shipping += OrderDetails[i].Shipping;
                }
                return shipping;
            }
        }

        public decimal Total
        {
            get
            {
                return Subtotal + Shipping;
            }
        }
    }
}
