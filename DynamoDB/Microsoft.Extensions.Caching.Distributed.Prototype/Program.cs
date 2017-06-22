using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
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
            StartUp();

            var someObject = new SampleDataModel
            {
                Id = 3,
                FirstName = "Tom",
                LastName = "Hardy",
                CompanyName = "Reed on-line"
            };

            var repositoryInstance = _serviceProvider.GetService<ISampleRepository>();

            repositoryInstance.Save(someObject);

            for (var count = 0; count < 1; count++)
            {
                var stopwatch =  Stopwatch.StartNew();

                var result = repositoryInstance.Get(someObject.Id);

                Console.WriteLine($"{result.FirstName}, {result.LastName} ({stopwatch.ElapsedMilliseconds} ms)");

                stopwatch.Stop();
            }

            Console.ReadLine();
        }

        #region Set up IOC

        static void StartUp()
        {
            //Set up the variables
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

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        #endregion
    }
}
