using Newtonsoft.Json;
using System;
using System.Web;

namespace Deerfly_Patches.Modules
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this HttpSessionStateBase session, string key, object value)
        {
            session[key] = JsonConvert.SerializeObject(value);
        }

        public static T GetObjectFromJson<T>(this HttpSessionStateBase session, string key)
        {
            var value = session[key] as String;
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
