using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Deerfly_Patches.Modules.Google;
using Newtonsoft.Json;

namespace Deerfly_Patches.Tests
{
    [TestClass]
    public class GoogleMapsClientTest
    {
        private GoogleMapsClient _googleMapsClient = new GoogleMapsClient();

        [TestMethod]
        public void TestGetGeocodingResponse()
        {
            string result = "{\n   \"results\" : [\n      {\n         \"address_components\" : [\n            {\n               \"long_name\" : \"17852\",\n               \"short_name\" : \"17852\",\n               \"types\" : [ \"street_number\" ]\n            },\n            {\n               \"long_name\" : \"10 Mile Road\",\n               \"short_name\" : \"10 Mile Rd\",\n               \"types\" : [ \"route\" ]\n            },\n            {\n               \"long_name\" : \"Le Roy\",\n               \"short_name\" : \"Le Roy\",\n               \"types\" : [ \"locality\", \"political\" ]\n            },\n            {\n               \"long_name\" : \"Cedar Township\",\n               \"short_name\" : \"Cedar Township\",\n               \"types\" : [ \"administrative_area_level_3\", \"political\" ]\n            },\n            {\n               \"long_name\" : \"Osceola County\",\n               \"short_name\" : \"Osceola County\",\n               \"types\" : [ \"administrative_area_level_2\", \"political\" ]\n            },\n            {\n               \"long_name\" : \"Michigan\",\n               \"short_name\" : \"MI\",\n               \"types\" : [ \"administrative_area_level_1\", \"political\" ]\n            },\n            {\n               \"long_name\" : \"United States\",\n               \"short_name\" : \"US\",\n               \"types\" : [ \"country\", \"political\" ]\n            },\n            {\n               \"long_name\" : \"49655\",\n               \"short_name\" : \"49655\",\n               \"types\" : [ \"postal_code\" ]\n            },\n            {\n               \"long_name\" : \"8025\",\n               \"short_name\" : \"8025\",\n               \"types\" : [ \"postal_code_suffix\" ]\n            }\n         ],\n         \"formatted_address\" : \"17852 10 Mile Rd, Le Roy, MI 49655, USA\",\n         \"geometry\" : {\n            \"bounds\" : {\n               \"northeast\" : {\n                  \"lat\" : 43.9590503,\n                  \"lng\" : -85.44075119999999\n               },\n               \"southwest\" : {\n                  \"lat\" : 43.9590368,\n                  \"lng\" : -85.4407515\n               }\n            },\n            \"location\" : {\n               \"lat\" : 43.9590368,\n               \"lng\" : -85.44075119999999\n            },\n            \"location_type\" : \"RANGE_INTERPOLATED\",\n            \"viewport\" : {\n               \"northeast\" : {\n                  \"lat\" : 43.9603925302915,\n                  \"lng\" : -85.43940236970849\n               },\n               \"southwest\" : {\n                  \"lat\" : 43.9576945697085,\n                  \"lng\" : -85.44210033029151\n               }\n            }\n         },\n         \"place_id\" : \"EicxNzg1MiAxMCBNaWxlIFJkLCBMZSBSb3ksIE1JIDQ5NjU1LCBVU0E\",\n         \"types\" : [ \"street_address\" ]\n      }\n   ],\n   \"status\" : \"OK\"\n}\n";
            GeocodingResponse resultObj = _googleMapsClient.GetGeocodingResponseObject(result);

            Assert.IsNotNull(resultObj);
        }
    }
}
