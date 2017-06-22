using System;
using System.Text;
using Amazon;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Prototype.Repository;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Caching.Distributed.DynamoDb.Prototype
{
    class Program
    {
        #region Private variables

        private static IServiceProvider _serviceProvider;

        #endregion

        static void Main(string[] args)
        {
            //Run startup.
            StartUp();

            //Sample data to be stored in cache.
            var someObject = new SampleDataModel
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Tom",
                LastName = "Hardy",
                CompanyName = "Microsoft corporation"
            };

            //Get instance of sample repository from IOC
            var repositoryInstance = _serviceProvider.GetService<ISampleRepository>();

            //Call save method. (Saves to cache)
            repositoryInstance.Save(someObject);

            //Call get method. (Gets from cache)
            var result = repositoryInstance.Get(someObject.Id);

            Console.WriteLine($"{result.FirstName}, {result.LastName}");

            Console.ReadLine();
        }

        #region Set up IOC

        static void StartUp()
        {
            //Set up the variables (Load from configuration files)
            //Sensitive information can be stored in your deployment pipeline.
            var accessKey = "";
            var accessSecret = "";
            var region = RegionEndpoint.GetBySystemName("eu-west-2");
            var encoding = Encoding.GetEncoding("us-ascii");

            //Set up the IOC
            IServiceCollection serviceCollection = new ServiceCollection();

            //Register caching
            serviceCollection.RegisterDynomoDbCacheService<CustomCacheTable>(new DistributedCacheDynamoDbSettings(accessKey, accessSecret, encoding, region));

            //Register repository
            serviceCollection.Add(new ServiceDescriptor(typeof(ISampleRepository), typeof(SampleRepository), ServiceLifetime.Transient));

            //Initialize the IOC
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        #endregion
    }
}
