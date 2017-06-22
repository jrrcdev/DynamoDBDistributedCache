# Distributed Cache (IDistributedCache) implementation for DynamoDb (AWS)

### Setup.

Add the following nugets:

> * Microsoft.Extensions.DependencyInjection.Abstraction;
> * Microsoft.Extensions.DependencyInjection;
> * Microsoft.Extensions.Caching.Abstractions;
> * AWSSDK.Core;
> * AWSSDK.DynamoDBv2
> * Newtonsoft.Json

Registering the implementation in the IOC

```C# 
//Set up the variables (Load from configuration files)
//Sensitive information can be stored in your deployment pipeline.
var accessKey = "{AWS access key}";
var accessSecret = "{AWS access secret}";
var region = RegionEndpoint.GetBySystemName("eu-west-2");
var encoding = Encoding.GetEncoding("us-ascii");

//Sample data model which is to saved in cache
public class SampleDataModel
{
    public string Id;
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CompanyName { get; set; }
}

//Sample data to be stored in cache.
var someObject = new SampleDataModel
{
    Id = Guid.NewGuid().ToString(),
    FirstName = "Tom",
    LastName = "Hardy",
    CompanyName = "Microsoft corporation"
};

//Set up the IOC
IServiceCollection serviceCollection = new ServiceCollection();

//Register caching
serviceCollection.RegisterDynomoDbCacheService(new DistributedCacheDynamoDbSettings(accessKey, accessSecret, encoding, region));

//Register repository
serviceCollection.Add(new ServiceDescriptor(typeof(ISampleRepository), typeof(SampleRepository), ServiceLifetime.Transient));

//Initialize the IOC
_serviceProvider = serviceCollection.BuildServiceProvider();

//Get the cache instance
 var _distributedCache = _serviceProvider.GetService<IDistributedCache>();
 
 //Save to cache
 _distributedCache.SetObject(model.Id, model);
 
 //Read from cache
 _distributedCache.GetObject<SampleDataModel>(id); 
 
```

