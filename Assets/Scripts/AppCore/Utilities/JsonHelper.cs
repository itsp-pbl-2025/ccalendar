using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AppCore.Utilities
{
    public static class JsonHelper
    {
        [Serializable]
        private class Datum<T>
        {
            public T data;
        }
        
        public static string ToJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJson<T>(string json)
        {
            if (typeof(T).IsArray || (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>)))
            {
                return JsonConvert.DeserializeObject<Datum<T>>($"{{\"data\":{json}}}").data;
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
    }
}