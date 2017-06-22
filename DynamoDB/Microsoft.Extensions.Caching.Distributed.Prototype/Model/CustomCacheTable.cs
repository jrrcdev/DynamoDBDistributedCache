using Microsoft.Extensions.Caching.Distributed.DynamoDb.Models;

namespace Microsoft.Extensions.Caching.Distributed.DynamoDb.Prototype
{
    public class CustomCacheTable : ICacheTable
    {
        public string CacheId { get; set; }
        public string Value { get; set; }
        public long Ttl { get; set; }
        public CacheOptions CacheOptions { get; set; }
    }
}