using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3.Model;
using DataServiceCore;
using Services;

namespace AWSDataServices
{
    public class DynamoDBDataService : IGenericDataService, IMatchDataService
    {
        public IDiagnosticService DiagnosticService { get; set; }

        public DynamoDBDataService(IDiagnosticService diagnosticService)
        {
            this.DiagnosticService = diagnosticService;
        }

        public async Task StoreMatchData(MatchData matchData)
        {
            await DiagnosticService.ExecAsync(nameof(DynamoDBDataService), async diagnosticService =>
            {

                var dynamoClient = new AmazonDynamoDBClient(Config.CurrentRegion);
                matchData.AccountID = $"STM:{matchData.ClientInfo.SteamId}";
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
            });
        }
    }
}
