using Cstieg.Geography;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeerflyPatches.Models
{
    public class Retailer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [ForeignKey("Address")]
        public virtual int AddressId { get; set; }
        public virtual AddressBase Address { get; set; }

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