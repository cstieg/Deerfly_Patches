using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deerfly_Patches.Models
{
    public class Customer
    {
        public Customer()
        {
            Addresses = new List<Address>();
        }

        [Key]
        public int CustomerId { get; set; }

        [StringLength(50, MinimumLength = 4)]
        [Required]
        public string CustomerName { get; set; }

        public DateTime Registered { get; set; }

        public DateTime LastVisited { get; set; }

        public int TimesVisited { get; set; }

        public string EmailAddress { get; set; }

        [InverseProperty("Customer")]
        public virtual ICollection<Address> Addresses { get; set; }

        public override string ToString()
        {
            return CustomerName;
        }
    }

}
