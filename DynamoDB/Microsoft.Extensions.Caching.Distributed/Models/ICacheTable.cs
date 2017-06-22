namespace Microsoft.Extensions.Caching.Distributed.DynamoDb.Models
{
    public interface ICacheTable
    { 
        string CacheId { get; set; }

        string Value { get; set; }

        long Ttl { get; set; }

        CacheOptions CacheOptions { get; set; }
    }
}