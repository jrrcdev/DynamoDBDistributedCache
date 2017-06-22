namespace Microsoft.Extensions.Caching.Distributed.DynamoDb.Models
{
    public class CacheOptions
    {
        public string Type { get; set; }
        public long Span { get; set; }
    }
}