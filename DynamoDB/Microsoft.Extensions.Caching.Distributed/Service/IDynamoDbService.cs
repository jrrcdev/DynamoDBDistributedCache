namespace Microsoft.Extensions.Caching.Distributed.DynamoDb.Service
{
    public interface IDynamoDbService
    {
        bool CreateDb(string databaseName, int readCapacityUnits, int writeCapacityUnits);
        bool DeleteDb(string databaseName);
    }
}