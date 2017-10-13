using Deerfly_Patches.Modules.Geography;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deerfly_Patches.Models
{
    public class Address : Modules.Geography.AddressBase
    {
        [Key]
        public int AddressId { get; set; }

        [ForeignKey("Customer")]
        public int ?CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [StringLength(50, MinimumLength = 4)]
        public override string Recipient { get; set; }

        [StringLength(50, MinimumLength = 4)]
        public override string Address1 { get; set; }

        [StringLength(50)]
        public override string Address2 { get; set; }

        [StringLength(50)]
        public override string City { get; set; }

        [StringLength(50)]
        public override string State { get; set; }

        [StringLength(15)]
        public override string PostalCode { get; set; }

        [StringLength(50)]
        public override string Country { get; set; }

        [StringLength(25)]
        public override string Phone { get; set; }

        public override AddressType Type { get; set; }
    }
}