using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiKala.Core.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session,string key,object value)//this is because it will acting on object itselft like: obj.SetObject(key,value)
        {
            session.SetString(key,JsonConvert.SerializeObject(value));
        }
        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
