using System.Runtime.Serialization;

namespace Deerfly_Patches.Modules.PayPal
{
    [DataContract]
    public class PayPalAddress
    {
        [DataMember(Name = "street_address")]
        public string StreetAddress { get; set; }

        [DataMember(Name = "locality")]
        public string City { get; set; }

        [DataMember (Name = "region")]
        public string State { get; set; }

        [DataMember (Name = "postal_code")]
        public string PostalCode { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }
    }
}