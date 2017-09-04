using Newtonsoft.Json;
using System;
using System.Web;

namespace Deerfly_Patches.Modules
{
    /// <summary>
    /// Helper class to add extensions to Session
    /// </summary>
    public static class SessionHelper
    {
        /// <summary>
        /// Serializes an object to the session
        /// </summary>
        /// <param name="key">The key name in the session under which to store the object</param>
        /// <param name="value">The object to be serialized</param>
        public static void SetObjectAsJson(this HttpSessionStateBase session, string key, object value)
        {
            session[key] = JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Deserializes an object from the session
        /// </summary>
        /// <typeparam name="T">Original type of object</typeparam>
        /// <param name="key">The key name in the session under which the object was stored</param>
        /// <returns>The deserialized object</returns>
        public static T GetObjectFromJson<T>(this HttpSessionStateBase session, string key)
        {
            var value = session[key] as String;
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
