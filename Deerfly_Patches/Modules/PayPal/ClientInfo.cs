using System.Runtime.Serialization;

namespace Deerfly_Patches.Modules.PayPal
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

        [DataMember(Name = "express_checkout_access_token")]
        public string ExpressCheckoutAccessToken { get; set; }

        [DataMember(Name = "return_url")]
        public string ReturnUrl { get; set; }

        [DataMember(Name = "mode")]
        public string Mode { get; set; }
    }
}
