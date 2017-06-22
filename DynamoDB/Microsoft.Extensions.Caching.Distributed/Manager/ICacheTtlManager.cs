using System;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Models;

namespace Microsoft.Extensions.Caching.Distributed.DynamoDb.Manager
{
    public interface ICacheTtlManager
    {
        long ToUnixTime(DateTime date);
        CacheOptions ToCacheOptions(DistributedCacheEntryOptions options);
        long ToTtl(CacheOptions cacheOptions);
    }
}
