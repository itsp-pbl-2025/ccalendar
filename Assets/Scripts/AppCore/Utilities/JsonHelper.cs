using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace AppCore.Utilities
{
    public static class JsonHelper
    {
        public static string ToJson<T>(T obj)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using var ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static T FromJson<T>(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var bytes = Encoding.UTF8.GetBytes(json);
            using var ms = new MemoryStream(bytes);
            return (T)serializer.ReadObject(ms)!;
        }
    }
}