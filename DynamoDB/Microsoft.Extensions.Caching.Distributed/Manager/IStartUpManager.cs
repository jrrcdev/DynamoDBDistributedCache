namespace Microsoft.Extensions.Caching.Distributed.DynamoDb.Manager
{
    public interface IStartUpManager
    {
        void Run(string tableName);
    }
}
