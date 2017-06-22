using System.Collections.Generic;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Caching.Distributed.DynamoDb.Models;

namespace Microsoft.Extensions.Caching.Distributed.DynamoDb.Service
{
    public class DynamoDbService : IDynamoDbService
    {   
        private readonly IAmazonDynamoDB _amazonDynamoDb;

        public DynamoDbService(IAmazonDynamoDB amazonDynamoDb)
        {
            _amazonDynamoDb = amazonDynamoDb;
        }

        public bool CreateDb(string tableName, int readCapacityUnits, int writeCapacityUnits)
        {
            try
            {
                _amazonDynamoDb.DescribeTableAsync(tableName).GetAwaiter().GetResult();

                return false;
            }
            catch (ResourceNotFoundException)
            {
                try
                {
                    var tableDescription = _amazonDynamoDb.CreateTableAsync(new CreateTableRequest
                    {
                        TableName = tableName,
                        AttributeDefinitions = new List<AttributeDefinition>()
                        {
                            new AttributeDefinition
                            {
                                AttributeName = nameof(ICacheTable.CacheId),
                                AttributeType = ScalarAttributeType.S
                            }
                        },
                        KeySchema = new List<KeySchemaElement>()
                        {
                            new KeySchemaElement
                            {
                                AttributeName = nameof(ICacheTable.CacheId),
                                KeyType = KeyType.HASH
                            }
                        },
                        ProvisionedThroughput = new ProvisionedThroughput
                        {
                            ReadCapacityUnits = readCapacityUnits,
                            WriteCapacityUnits = writeCapacityUnits
                        }
                    }).GetAwaiter().GetResult().TableDescription;

                    var status = tableDescription.TableStatus;

                    while (status != TableStatus.ACTIVE)
                    {
                        System.Threading.Thread.Sleep(5000);

                        var describeTableResponse = _amazonDynamoDb.DescribeTableAsync(new DescribeTableRequest { TableName = tableName }).GetAwaiter().GetResult();

                        status = describeTableResponse.Table.TableStatus;
                    }

                    var updateTimeToLiveRequest = new UpdateTimeToLiveRequest
                    {
                        TableName = tableName,
                        TimeToLiveSpecification = new TimeToLiveSpecification
                        {
                            AttributeName = nameof(ICacheTable.Ttl),
                            Enabled = true
                        }
                    };

                    _amazonDynamoDb.UpdateTimeToLiveAsync(updateTimeToLiveRequest).GetAwaiter().GetResult();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool DeleteDb(string databaseName)
        {
            var status = _amazonDynamoDb.DescribeTableAsync(databaseName).GetAwaiter().GetResult().Table.TableStatus;

            if (status == TableStatus.ACTIVE)
            {
                var result = _amazonDynamoDb.DeleteTableAsync(databaseName).GetAwaiter().GetResult();

                return result.HttpStatusCode == HttpStatusCode.OK;
            }

            return false;
        }
    }
}
