namespace Microsoft.Extensions.Caching.Distributed.DynamoDb.Prototype.Repository
{
    public interface ISampleRepository
    {
        void Save(SampleDataModel model);

        SampleDataModel Get(string id);
    }
}