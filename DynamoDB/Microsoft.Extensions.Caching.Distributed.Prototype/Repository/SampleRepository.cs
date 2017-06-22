namespace Microsoft.Extensions.Caching.Distributed.DynamoDb.Prototype.Repository
{
    public class SampleRepository : ISampleRepository
    {
        private readonly IDistributedCache _distributedCache;

        public SampleRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public void Save(SampleDataModel model)
        {
            _distributedCache.SetObject(model.Id.ToString(), model);
        }

        public SampleDataModel Get(int id)
        {
            return _distributedCache.GetObject<SampleDataModel>(id.ToString());
        }
    }
}
