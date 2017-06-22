using Microsoft.Extensions.Caching.Distributed.DynamoDb.Service;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Settings;

namespace Microsoft.Extensions.Caching.Distributed.DynamoDb.Manager
{
    public class StartUpManager: IStartUpManager
    {
        private readonly IDynamoDbService _dynamoDb;
        private readonly DistributedCacheDynamoDbStartUpSettings _distributedCacheDynamoDbStartUpSettings;
        public StartUpManager(IDynamoDbService dynamoDb, DistributedCacheDynamoDbStartUpSettings distributedCacheDynamoDbStartUpSettings)
        {
            _dynamoDb = dynamoDb;
            _distributedCacheDynamoDbStartUpSettings = distributedCacheDynamoDbStartUpSettings;
        }
        public void Run(string tableName)
        {
            if(_distributedCacheDynamoDbStartUpSettings != null && _distributedCacheDynamoDbStartUpSettings.CreateDbOnStartUp)
            {
                _dynamoDb.CreateDb(tableName, _distributedCacheDynamoDbStartUpSettings.ReadCapacityUnits, _distributedCacheDynamoDbStartUpSettings.WriteCapacityUnits);
            }
        }
    }
}
