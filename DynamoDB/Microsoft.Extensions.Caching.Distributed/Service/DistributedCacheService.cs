using System;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Constants;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Manager;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Models;

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

        public Task<byte[]> GetAsync(string key)
        {
            return Task.FromResult(Get(key));
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

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            Set(key, value, options);

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

        public Task RefreshAsync(string key)
        {
            Refresh(key);

            return Task.FromResult(0);
        }

        public void Remove(string key)
        {
            _dynamoDbContext.DeleteAsync<T>(key).GetAwaiter().GetResult();
        }

        public Task RemoveAsync(string key)
        {
            Remove(key);

            return Task.FromResult(0);
        }
    }
}
