using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Manager;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Models;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Service;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Caching.Distributed.DynamoDb
{
    public static class Registration
    {
        public static void RegisterDynomoDbCacheService(this IServiceCollection services, DistributedCacheDynamoDbSettings settings)
        {
            RegisterDynomoDbCacheService<DefaultCacheTable>(services, settings, ServiceLifetime.Scoped);
        }

        public static void RegisterDynomoDbCacheService<T>(this IServiceCollection services, DistributedCacheDynamoDbSettings settings) where T : ICacheTable
        {
            RegisterDynomoDbCacheService<T>(services, settings, ServiceLifetime.Scoped);
        }

        public static void RegisterDynomoDbCacheService(this IServiceCollection services, DistributedCacheDynamoDbSettings settings, ServiceLifetime lifeTime)
        {
            RegisterDynomoDbCacheService<DefaultCacheTable>(services, settings, lifeTime);
        }

        public static void RegisterDynomoDbCacheService<T>(this IServiceCollection services, DistributedCacheDynamoDbSettings settings, ServiceLifetime lifeTime) where T : ICacheTable
        {
            services.Add(new ServiceDescriptor(typeof(ICacheTtlManager), (c) => new CacheTtlManager(settings.DefaultTtl), lifeTime));

            services.Add(new ServiceDescriptor(typeof(IDynamoDBContext), (c) => new DynamoDBContext(new AmazonDynamoDBClient(new BasicAWSCredentials(settings.AccessKey, settings.AccessSecret),
                new AmazonDynamoDBConfig { RegionEndpoint = settings.ReginEndpoint })), lifeTime));

            services.Add(new ServiceDescriptor(typeof(IAmazonDynamoDB), (c) => new AmazonDynamoDBClient(new BasicAWSCredentials(settings.AccessKey, settings.AccessSecret),
                new AmazonDynamoDBConfig { RegionEndpoint = settings.ReginEndpoint }), lifeTime));

            services.Add(new ServiceDescriptor(typeof(IDynamoDbService), typeof(DynamoDbService), lifeTime));

            services.Add(new ServiceDescriptor(typeof(IStartUpManager), (c) => new StartUpManager(c.GetService<IDynamoDbService>(), settings.StartUpSettings), lifeTime));

            services.Add(new ServiceDescriptor(typeof(IDistributedCache),
                (c) => new DistributedCacheService<T>(c.GetService<IDynamoDBContext>(), c.GetService<ICacheTtlManager>(), c.GetService<IStartUpManager>(), settings.Encoding), lifeTime));
        }
    }
}
