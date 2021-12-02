using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DataServiceCore;
using Services;

namespace AWSDataServices
{
    public class DynamoDBDataService : IGenericDataService, IMatchDataService
    {
        public async Task StoreMatchData(MatchData matchData)
        {
            var dynamoClient = new AmazonDynamoDBClient(RegionEndpoint.EUWest1);
            matchData.AccountID = $"STM:{matchData.ClientInfo.SteamId}";
            matchData.ItemID = Guid.NewGuid().ToString("N");

            var matchDataJson = ServiceCore.ToJsonString(matchData);
            var matchDataDoc = Document.FromJson(matchDataJson);
            
            var runtimeEnvironment= ServiceCore.GetRuntimeEnvironment();
            var tableName = $"ibotsota-Account-{runtimeEnvironment}";

            var table = Table.LoadTable(dynamoClient, tableName);
            await table.PutItemAsync(matchDataDoc);
        }
    }
}
