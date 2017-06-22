namespace Microsoft.Extensions.Caching.Distributed.DynamoDb.Constants
{
    public class CacheExpiryType
    {
        public static string Absolute => "Absolute";
        public static string Sliding => "Sliding";
    }
}