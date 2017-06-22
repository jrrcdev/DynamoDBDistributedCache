namespace Microsoft.Extensions.Caching.Distributed.DynamoDb.Prototype.Repository
{
    public class SampleRepository : ISampleRepository
    {
        private readonly IDistributedCache _distributedCache;

        public SampleRepository(IDistributedCache distributedCache)
        {
            //Distributed cache instance
            _distributedCache = distributedCache;
        }

        public void Save(SampleDataModel model)
        {
            //Save to cache
            _distributedCache.SetObject(model.Id.ToString(), model);
        }

        public SampleDataModel Get(string id)
        {
            //Get from cache
            return _distributedCache.GetObject<SampleDataModel>(id.ToString());
        }
    }
}
