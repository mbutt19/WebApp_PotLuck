using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Potluck.Web.Infrastructure
{
    /// <summary>
    /// Helper class for the session
    /// Saves data as json in session
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Stores c# object as json in seesion
        /// </summary>
        /// <param name="session">ISession</param>
        /// <param name="key">String</param>
        /// <param name="value">Object</param>
        public static void SetJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// Gets session data converting from json to c# object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="session">ISession</param>
        /// <param name="key">String</param>
        /// <returns></returns>
        public static T GetJson<T>(this ISession session, string key)
        {
            var sessionData = session.GetString(key);

            return sessionData == null
                ? default(T)
                : JsonConvert.DeserializeObject<T>(sessionData);
        }


    }
}
