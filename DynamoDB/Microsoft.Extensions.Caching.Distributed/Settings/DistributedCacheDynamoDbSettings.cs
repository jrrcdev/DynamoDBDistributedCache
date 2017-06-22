using System.Text;
using Amazon;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Constants;

namespace Microsoft.Extensions.Caching.Distributed.DynamoDb.Settings
{
    public class DistributedCacheDynamoDbSettings
    {
        /// <summary>
        /// AWS Access key
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// AWS Secret
        /// </summary>
        public string AccessSecret { get; set; }

        /// <summary>
        /// AWS Region
        /// </summary>
        public RegionEndpoint ReginEndpoint { get; set; }

        /// <summary>
        /// Default time to live for cache items
        /// </summary>
        public long DefaultTtl { get; set; }

        /// <summary>
        /// Encoding of data
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Start up settings
        /// </summary>
        public DistributedCacheDynamoDbStartUpSettings StartUpSettings { get; set; }

        /// <summary>
        /// Settings for Dynamo db cache provider
        /// </summary>
        /// <param name="accessKey"></param>
        /// <param name="accessSecret"></param>
        /// <param name="encoding"></param>
        /// <param name="reginEndpoint"></param>
        public DistributedCacheDynamoDbSettings(string accessKey, string accessSecret, Encoding encoding, RegionEndpoint reginEndpoint)
        {
            AccessKey = accessKey;

            AccessSecret = accessSecret;

            ReginEndpoint = reginEndpoint;

            Encoding = encoding;

            DefaultTtl = CacheTableAttributes.Ttl;

            StartUpSettings = new DistributedCacheDynamoDbStartUpSettings
            {
                ReadCapacityUnits = CacheTableAttributes.ReadCapacityUnits,
                WriteCapacityUnits = CacheTableAttributes.WriteCapacityUnits,
                CreateDbOnStartUp = CacheTableAttributes.CreateTableOnStartUp
            };
        }

    }
}
