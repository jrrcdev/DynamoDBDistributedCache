using Newtonsoft.Json;

namespace Microsoft.Extensions.Caching.Distributed.DynamoDb
{
    public static class Extensions
    {
        public static T GetObject<T>(this IDistributedCache cache, string key)
        {
            var result = cache.GetString(key);

            return result != null ? JsonConvert.DeserializeObject<T>(result) : default(T);
        }

        public static void SetObject<T>(this IDistributedCache cache, string key, T value)
        {
            cache.SetString(key, JsonConvert.SerializeObject(value));
        }

    }
}
