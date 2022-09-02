using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Constants;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Manager;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Models;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.Distributed.DynamoDb.Service
{
    public class DistributedCacheService<T> : IDistributedCache where T : ICacheTable
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ICacheTtlManager _cacheTtlManager;
        private readonly Encoding _encoding;
        public DistributedCacheService(IDynamoDBContext dynamoDbContext, ICacheTtlManager cacheTtlManager, IStartUpManager startUpManager, Encoding encoding)
        {
            _dynamoDbContext = dynamoDbContext;

            _cacheTtlManager = cacheTtlManager;

            _encoding = encoding;

            startUpManager.Run(typeof(T).Name);
        }

        public byte[] Get(string key)
        {
            var cacheItem = _dynamoDbContext.LoadAsync<T>(key).GetAwaiter().GetResult();

            if (cacheItem == null)
                return null;

            if (cacheItem.Ttl < _cacheTtlManager.ToUnixTime(DateTime.UtcNow))
            {
                Remove(cacheItem.CacheId);
                return null;
            }

            return _encoding.GetBytes(_dynamoDbContext.LoadAsync<T>(key).GetAwaiter().GetResult().Value);
        }

        public byte[] Get(string key, CancellationToken token = default)
        {
            var cacheItem = _dynamoDbContext.LoadAsync<T>(key, token).GetAwaiter().GetResult();

            if (cacheItem == null)
                return null;

            if (cacheItem.Ttl < _cacheTtlManager.ToUnixTime(DateTime.UtcNow))
            {
                Remove(cacheItem.CacheId);
                return null;
            }

            return _encoding.GetBytes(_dynamoDbContext.LoadAsync<T>(key, token).GetAwaiter().GetResult().Value);
        }

        public Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            return Task.FromResult(Get(key, token));
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            var cacheItem = Activator.CreateInstance<T>();

            cacheItem.CacheId = key;
            cacheItem.Value = _encoding.GetString(value);
            cacheItem.CacheOptions = _cacheTtlManager.ToCacheOptions(options);
            cacheItem.Ttl = _cacheTtlManager.ToTtl(cacheItem.CacheOptions);

            _dynamoDbContext.SaveAsync(cacheItem).GetAwaiter().GetResult();
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            var cacheItem = Activator.CreateInstance<T>();

            cacheItem.CacheId = key;
            cacheItem.Value = _encoding.GetString(value);
            cacheItem.CacheOptions = _cacheTtlManager.ToCacheOptions(options);
            cacheItem.Ttl = _cacheTtlManager.ToTtl(cacheItem.CacheOptions);

            _dynamoDbContext.SaveAsync(cacheItem, token).GetAwaiter().GetResult();
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            Set(key, value, options, token);

            return Task.FromResult(0);
        }

        public void Refresh(string key)
        {
            var cacheItem = _dynamoDbContext.LoadAsync<T>(key).GetAwaiter().GetResult();

            if (cacheItem != null)
            {
                if (cacheItem.CacheOptions.Type == CacheExpiryType.Sliding)
                {
                    cacheItem.Ttl = _cacheTtlManager.ToTtl(cacheItem.CacheOptions);
                }

                _dynamoDbContext.SaveAsync(cacheItem).GetAwaiter().GetResult();
            }
        }

        public void Refresh(string key, CancellationToken token = default)
        {
            var cacheItem = _dynamoDbContext.LoadAsync<T>(key, token).GetAwaiter().GetResult();

            if (cacheItem != null)
            {
                if (cacheItem.CacheOptions.Type == CacheExpiryType.Sliding)
                {
                    cacheItem.Ttl = _cacheTtlManager.ToTtl(cacheItem.CacheOptions);
                }

                _dynamoDbContext.SaveAsync(cacheItem, token).GetAwaiter().GetResult();
            }
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            Refresh(key, token);

            return Task.FromResult(0);
        }

        public void Remove(string key)
        {
            _dynamoDbContext.DeleteAsync<T>(key).GetAwaiter().GetResult();
        }

        public void Remove(string key, CancellationToken token = default)
        {
            _dynamoDbContext.DeleteAsync<T>(key, token).GetAwaiter().GetResult();
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            Remove(key, token);

            return Task.FromResult(0);
        }
    }
}
