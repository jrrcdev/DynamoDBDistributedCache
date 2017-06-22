namespace Microsoft.Extensions.Caching.Distributed.DynamoDb.Models
{
    public class DefaultCacheTable : ICacheTable
    {
        public string CacheId { get; set; }

        public string Value { get; set; }

        public long Ttl { get; set; }
        public CacheOptions CacheOptions { get; set; }
    }
}