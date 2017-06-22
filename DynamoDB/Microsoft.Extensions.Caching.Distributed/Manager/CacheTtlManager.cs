using System;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Constants;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Models;

namespace Microsoft.Extensions.Caching.Distributed.DynamoDb.Manager
{
    public class CacheTtlManager : ICacheTtlManager
    {
        private readonly long _ttl;

        public CacheTtlManager(long ttl)
        {
            _ttl = ttl;
        }

        public long ToUnixTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }
        public CacheOptions ToCacheOptions(DistributedCacheEntryOptions options)
        {
            if (options.AbsoluteExpiration != null)
            {
                return new CacheOptions
                {
                    Type = CacheExpiryType.Absolute,
                    Span = (long)(options.AbsoluteExpiration.Value.ToUniversalTime().DateTime - DateTimeOffset.UtcNow.DateTime).TotalMinutes
                };
            }

            if (options.AbsoluteExpirationRelativeToNow != null)
            {
                return new CacheOptions
                {
                    Type = CacheExpiryType.Absolute,
                    Span = (long)options.AbsoluteExpirationRelativeToNow.Value.TotalMinutes
                };
            }

            if (options.SlidingExpiration != null)
            {
                return new CacheOptions
                {
                    Type = CacheExpiryType.Sliding,
                    Span = (long)options.SlidingExpiration.Value.TotalMinutes
                };
            }

            return new CacheOptions
            {
                Type = CacheExpiryType.Absolute,
                Span = (long)(DateTimeOffset.UtcNow.AddMinutes(_ttl).DateTime - DateTimeOffset.UtcNow.DateTime).TotalMinutes
            };
        }

        public long ToTtl(CacheOptions cacheOptions)
        {
            return ToUnixTime(DateTime.UtcNow.AddMinutes(cacheOptions.Span));
        }
    }
}