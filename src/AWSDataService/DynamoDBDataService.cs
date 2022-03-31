using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Internal;
using Amazon.S3.Model;
using DataServiceCore;
using Services;

namespace AWSDataServices
{
    public class DynamoDBDataService : IGenericDataService, IMatchDataService
    {
        public async Task StoreMatchData(MatchData matchData)
        {

            var dynamoClient = new AmazonDynamoDBClient(Config.CurrentRegion);
            matchData.AccountID = getAccountID(matchData.ClientInfo.SteamId.ToString());
            var matchID = Guid.NewGuid().ToString("N");
            matchData.ItemID = matchID;

            var fullMatchDataJson = ServiceCore.ToJsonString(matchData);

            var reducedMatchData = ServiceCore.FromJson<MatchData>(fullMatchDataJson);
            reducedMatchData.ChamberDatas.ForEach(item => item.PositionalDatas.Clear());

            var reducedMatchDataJson = ServiceCore.ToJsonString(reducedMatchData);

            var matchDataDoc = Document.FromJson(reducedMatchDataJson);

            var runtimeEnvironment = ServiceCore.GetRuntimeEnvironment();
            var tableName = $"ibotsota-Account-{runtimeEnvironment}";

            var table = Table.LoadTable(dynamoClient, tableName);
            await table.PutItemAsync(matchDataDoc);

            var s3service = new S3DataService(Config.CurrentRegion);
            var binaryJson = Encoding.UTF8.GetBytes(fullMatchDataJson);
            var gzippedData = ServiceCore.GZipData(binaryJson);

            var bucketName = $"ibotsota-{runtimeEnvironment}";
            using (var memStream = new MemoryStream(gzippedData))
            {
                await s3service.S3Client.PutObjectAsync(new PutObjectRequest()
                {
                    BucketName = bucketName,
                    Key = $"MatchData-{matchID}.json.gz",
                    ContentType = "application/gzip",
                    InputStream = memStream
                });
            }
        }

        public async Task<string> GetMatchData(string steamId)
        {
            var accountID = getAccountID(steamId);
            var dynamoClient = new AmazonDynamoDBClient(Config.CurrentRegion);
            var runtimeEnvironment = ServiceCore.GetRuntimeEnvironment();
            var tableName = $"ibotsota-Account-{runtimeEnvironment}";

            var queryResponse = await dynamoClient.QueryAsync(new QueryRequest(tableName)
                {
                    KeyConditionExpression = $"AccountID=:accountID",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    {
                        { ":accountID", new AttributeValue(accountID) }
                    },
                });
            
            var resultItems = queryResponse.Items;

            var matchDataJsons = resultItems.Select(item => Document.FromAttributeMap(item).ToJson()).ToArray();

            var jsonCombined = String.Join(", ", matchDataJsons);

            var matchDatas = $"{{ \"MatchDatas\": [ {jsonCombined} ] }}";

            return matchDatas;
        }

        private string getAccountID(string steamId)
        {
            return $"STM:{steamId}";
        }
    }
}
