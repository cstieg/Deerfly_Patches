using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Runtime.Serialization;
using Deerfly_Patches.Models;

namespace Deerfly_Patches.Modules
{
    [DataContract]
    public class GeoLocation
    {
        [DataMember(Name = "ip")]
        public string Ip { get; set; }

        [DataMember(Name = "country_code")]
        public string CountryCode { get; set; }

        [DataMember(Name = "country_name")]
        public string CountryName { get; set; }

        [DataMember(Name = "region_code")]
        public string RegionCode { get; set; }

        [DataMember(Name = "region_name")]
        public string RegionName { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "zip_code")]
        public string ZipCode { get; set; }

        [DataMember(Name = "time_zone")]
        public string TimeZone { get; set; }

        [DataMember(Name = "latitude")]
        public float Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public float Longitude { get; set; }

        [DataMember(Name = "metro_code")]
        public string MetroCode { get; set; }

        public LatLng LatLng
        { 
            get
            {
                return new LatLng()
                {
                    Lat = Latitude,
                    Lng = Longitude
                };
            }
        }

        public async static Task<GeoLocation> GetGeoLocation()
        {
            using (var client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "http://freegeoip.net/json/");
                var response = await client.SendAsync(request);
                string result = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<GeoLocation>(result);
            }
            //"{\"ip\":\"107.77.253.6\",\"country_code\":\"US\",\"country_name\":\"United States\",\"region_code\":\"NY\",\"region_name\":\"New York\",\"city\":\"The Bronx\",\"zip_code\":\"10451\",\"time_zone\":\"America/New_York\",\"latitude\":40.8195,\"longitude\":-73.9209,\"metro_code\":501}\n"
        }
    }
}