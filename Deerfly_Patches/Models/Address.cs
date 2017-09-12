using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deerfly_Patches.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [ForeignKey("Customer")]
        public int ?CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [StringLength(50, MinimumLength = 4)]
        public string Recipient { get; set; }

        [StringLength(50, MinimumLength = 4)]
        public string Address1 { get; set; }

        [StringLength(50)]
        public string Address2 { get; set; }

        [StringLength(50, MinimumLength = 2)]
        public string City { get; set; }

        [StringLength(50, MinimumLength = 2)]
        public string State { get; set; }

        [StringLength(15, MinimumLength = 3)]
        public string Zip { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Country { get; set; }

        [StringLength(25, MinimumLength = 5)]
        public string Phone { get; set; }

        public AddressType Type { get; set; }
        
        public override string ToString()
        {
            return Address1 + " " + Address2 + ", " + City + ", " + State + " " + Zip;
        }
        
    }

    public enum AddressType
    {
        Billing,
        Shipping
    }
}