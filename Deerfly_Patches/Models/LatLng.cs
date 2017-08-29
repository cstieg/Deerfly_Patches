using System;
using System.ComponentModel.DataAnnotations;

namespace Deerfly_Patches.Models
{
    public class LatLng
    {
        [Key]
        public int LatLngId { get; set; }

        public float Lat { get; set; }

        public float Lng { get; set; }

        public LatLng()
        {

        }

        public LatLng(float lat, float lng)
        {
            Lat = lat;
            Lng = lng;
        }

        public string ToJson()
        {
            return "{ \"lat\": " + Lat.ToString() + ", \"lng\": " + Lng.ToString() + "}";
        }

        // Haversine formula
        public double MilesTo(LatLng point)
        {
            // pi / 180
            double p = 0.017453292519943295;
            
            var a = 0.5 - Math.Cos((point.Lat - this.Lat) * p) / 2 +
                          Math.Cos(this.Lat * p) * Math.Cos(point.Lat * p) *
                          (1 - Math.Cos((point.Lng - this.Lng) * p)) / 2;
            // 2 * R; R = 6371 km
            double km = 12742 * Math.Asin(Math.Sqrt(a));
            double miles = km * 0.621371;
            return miles;
        }
    }
}