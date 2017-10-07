using System;
using System.Runtime.Serialization;

namespace Deerfly_Patches.Modules.PayPal
{
    [DataContract]
    public class AccessTokenBase
    {
        public AccessTokenBase()
        {
            _created = DateTime.Now;
        }

        private DateTime _created;

        [DataMember(Name = "access_token")]
        public string AccessTokenString { get; set; }

        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        [DataMember(Name = "expires_in")]
        public string ExpiresIn { get; set; }

        public DateTime Expires
        {
            get
            {
                TimeSpan lifespanSeconds = new TimeSpan(0, 0, int.Parse(ExpiresIn));
                return _created + lifespanSeconds;
            }
        }

        public bool IsExpired
        {
            get
            {
                // consider to be expired if will expire in next 60 seconds
                return DateTime.Now + new TimeSpan(0, 0, 60) > Expires;
            }
        }
    }

    public class AccessToken : AccessTokenBase
    {
        public AccessToken() : base() { }

        [DataMember(Name = "scope")]
        public string Scope { get; set; }

        [DataMember(Name = "nonce")]
        public string Nonce { get; set; }

        [DataMember(Name = "app_id")]
        public string AppId { get; set; }
    }

    public class UserAccessToken : AccessTokenBase
    {
        public UserAccessToken() : base() { }

        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        [DataMember(Name = "id_token")]
        public string IdToken { get; set; }
    }
}
