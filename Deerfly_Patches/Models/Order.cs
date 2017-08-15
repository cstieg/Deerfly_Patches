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

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }

        [Required]
        public DateTime DateOrdered { get; set; }

        public int ShipToAddressId { get; set; }
        public Address ShipToAddress { get; set; }

        public int BillToAddressId { get; set; }
        public Address BillToAddress { get; set; }

        [InverseProperty("Order")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public decimal Subtotal { get; set; }
        
        public decimal Shipping { get; set; }

        public decimal Total
        {
            get
            {
                return Subtotal + Shipping;
            }
        }
    }
}
