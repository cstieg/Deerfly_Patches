using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Deerfly_Patches.Models
{
    public class Retailer
    {
        [Key]
        public int RetailerId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [ForeignKey("Address")]
        public virtual int AddressId { get; set; }
        public virtual Address Address { get; set; }

        [ForeignKey("LatLng")]
        public virtual int ?LatLngId { get; set; }
        public virtual LatLng LatLng { get; set; }

        [Url]
        public string Website { get; set; }

        public override string ToString()
        {
            return string.Join(",", new string[] { Name, Address.ToString() });
        }
    }
}