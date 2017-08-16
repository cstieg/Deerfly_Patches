using System.Runtime.Serialization;

namespace DeerflyPatches.Modules.PayPal
{
    [DataContract]
    public class ClientInfo
    {
        [DataMember(Name = "client_account")]
        public string ClientAccount { get; set; }

        [DataMember(Name = "client_id")]
        public string ClientId { get; set; }

        [DataMember(Name = "client_secret")]
        public string ClientSecret { get; set; }
    }
}
