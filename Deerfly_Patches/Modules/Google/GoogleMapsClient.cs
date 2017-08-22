using Deerfly_Patches.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Deerfly_Patches.Modules.Google
{
    public class GoogleMapsClient
    {
        private string _ApiKey = ConfigurationManager.AppSettings["GoogleMapsApiKey"];
        private string _baseUrl = "https://maps.googleapis.com/maps/api/";

        public async Task<LatLng> GeocodeAddress(Address address)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en_US"));

                // construct url for google maps api
                string url = _baseUrl;
                url += "geocode/json";
                url += "?address=" + address.ToString();
                url += "&key=" + _ApiKey;
                url = url.Replace(" ", "+");

                // construct request
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

                // send request and parse response
                var response = await client.SendAsync(request);
                var result = response.Content.ReadAsStringAsync().Result;
                GeocodingResponse geocodingResponse = JsonConvert.DeserializeObject<GeocodingResponse>(result);
                return geocodingResponse.Results.First<GeocodingResult>().Geometry.Location;
            }
        }

        // Deserialize json response into an object (nested classes) conforming to google maps api result format
        public GeocodingResponse GetGeocodingResponseObject(string result)
        {
            return JsonConvert.DeserializeObject<GeocodingResponse>(result);
        }
    }


    // Classes mapping google maps api call result
    public class GeocodingResponse
    {
        [JsonProperty("results")]
        public List<GeocodingResult> Results { get; set; }
    }


    public class GeocodingResult
    {
        [JsonProperty("address_components")]
        public List<AddressComponent> AddressComponents { get; set; }

        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty("partial_match")]
        public bool PartialMatch { get; set; }

        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        [JsonProperty("types")]
        public List<string> Types { get; set; }
    }

    public class AddressComponent
    {
        [JsonProperty("long_name")]
        public string LongName { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [JsonProperty("types")]
        public List<string> Types { get; set; }
    }

    public class Geometry
    {
        [JsonProperty("location")]
        public LatLng Location { get; set; }

        [JsonProperty("location_type")]
        public string LocationType { get; set; }

        [JsonProperty("viewport")]
        public Viewport Viewport { get; set; }
    }

    public class Viewport
    {
        [JsonProperty("northeast")]
        public LatLng Northeast { get; set; }

        [JsonProperty("southwest")]
        public LatLng Southwest { get; set; }
    }

}