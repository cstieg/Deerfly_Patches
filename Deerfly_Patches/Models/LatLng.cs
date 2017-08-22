using System.ComponentModel.DataAnnotations;

namespace Deerfly_Patches.Models
{
    public class LatLng
    {
        [Key]
        public int LatLngId { get; set; }

        public float Lat { get; set; }

        public float Lng { get; set; }
    }
}