using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Amazon;
using Amazon.APIGateway;
using Amazon.APIGateway.Model;
using AWSDataServices;
using DataServiceCore;
using Services;
using DryIoc;
using iBotSotALambda.Controllers;
using Microsoft.AspNetCore.Mvc;
using SteamServices;
using Xunit;
using HexUtil = iBotSotALambda.Controllers.HexUtil;

namespace iBotSotALambda.Tests
{
    public class DataServiceControllerTests : IAsyncLifetime
    {
        private uint SteamAppId;
        private string SteamWebApiKey;
        private string LambdaEndpointUrl;

        [Fact]
        public async Task SelfAuthenticationTest()
        {
            try
            {
                var container = new Container();
                container.Register<IDiagnosticService, NoOpDiagnosticService>(Reuse.Singleton);
                container.Register<ISteamService, SteamService>(Reuse.Singleton);
                var steamService = container.Resolve<ISteamService>();
                steamService.InitService(SteamAppId, SteamWebApiKey);
                steamService.InitSteamClient();

                var authData = await steamService.GetAuthTokenA();
                var authenticated = await steamService.ValidateAuthToken(authData.steamIdValue, authData.authToken);
                Assert.True(authenticated);
            }
            catch (Exception ex)
            {

            }
        }

        [Fact]
        public async Task SelfAuthenticationTestWeb()
        {
            var container = new Container();
            container.Register<IDiagnosticService, NoOpDiagnosticService>(Reuse.Singleton);
            container.Register<ISteamService, SteamService>(Reuse.Singleton);

            var steamService = container.Resolve<ISteamService>();
            steamService.InitService(SteamAppId, SteamWebApiKey);
            steamService.InitSteamClient();

            var authData = await steamService.GetAuthTokenA();
            var authDataHex = HexUtil.ToHexString(authData.authToken);
            var authenticated = await steamService.ValidateAuthTokenWeb(authDataHex);
            Assert.True(authenticated.isAuthenticated);
        }


        [Fact]
        public async Task ServerAuthenticateTest()
        {
            var container = new Container();
            container.Register<IDiagnosticService, NoOpDiagnosticService>(Reuse.Singleton);
            container.Register<ISteamService, SteamService>(Reuse.Singleton);
            container.Register<IMatchDataService, DynamoDBDataService>(Reuse.Singleton);

            var steamService = container.Resolve<ISteamService>();
            steamService.InitService(SteamAppId, SteamWebApiKey);

            var matchDataService = container.Resolve<IMatchDataService>();

            steamService.InitSteamClient();
            var authData = await steamService.GetAuthTokenA();

            var controller = new DataServiceController(steamService, matchDataService);
            var authDataHex = HexUtil.ToHexString(authData.authToken);
            //var result = (JsonResult) await controller.AuthTest(authData.steamIdValue, authDataHex);
            var result = (JsonResult) await controller.GetSteamAuthentication(authDataHex);
            var content = result.Value.ToString();
            Assert.StartsWith("{ isAuthenticated = True, steamId = ", content);
            Assert.EndsWith(", vacBanned = False, publisherBanned = False }", content);
        }

        [Fact]
        public async Task SteamKeyValidationTest()
        {
            var container = new Container();
            container.Register<IDiagnosticService, NoOpDiagnosticService>(Reuse.Singleton);
            container.Register<ISteamService, SteamService>(Reuse.Singleton);

            var steamService = container.Resolve<ISteamService>();
            steamService.InitService(SteamAppId, SteamWebApiKey);

            steamService.InitSteamClient();
            var authData = await steamService.GetAuthTokenA();

            using var httpClient = new HttpClient();
            var authDataHex = HexUtil.ToHexString(authData.authToken);
            
            var url = $"https://partner.steam-api.com/ISteamUserAuth/AuthenticateUserTicket/v1/?key={SteamWebApiKey}&appid={SteamAppId}&ticket={authDataHex}";
            var response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            Assert.StartsWith("{\"response\":{\"params\":{\"result\":\"OK\",", content);
            Assert.EndsWith("\",\"vacbanned\":false,\"publisherbanned\":false}}}", content);
        }

        [Fact]
        [Trait("TestType", "Integration")]
        public async Task DevServerAuthenticateJsonTest()
        {
            var container = new Container();
            container.Register<IDiagnosticService, NoOpDiagnosticService>(Reuse.Singleton);
            container.Register<ISteamService, SteamService>(Reuse.Singleton);

            var steamService = container.Resolve<ISteamService>();
            steamService.InitService(SteamAppId, SteamWebApiKey);

            steamService.InitSteamClient();
            var authData = await steamService.GetAuthTokenA();

            using var httpClient = new HttpClient();
            var authDataHex = HexUtil.ToHexString(authData.authToken);
            //var url = $"{LambdaEndpointUrl}/api/DataService/GetSteamAuthentication?authDataHex={authDataHex}";
            var url = $"https://lambda-dev.ibotsota.net/api/DataService/GetSteamAuthentication?authDataHex={authDataHex}";
            var response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            
            
            Assert.StartsWith("{\"isAuthenticated\":true,\"steamId\":", content);
        }


        [Fact]
        public async Task SendGameDataTest()
        {
            var container = new Container();
            container.Register<IDiagnosticService, NoOpDiagnosticService>(Reuse.Singleton);
            container.Register<ISteamService, SteamService>(Reuse.Singleton);
            container.Register<IMatchDataService, DynamoDBDataService>(Reuse.Singleton);

            var steamService = container.Resolve<ISteamService>();
            steamService.InitService(SteamAppId, SteamWebApiKey);
            steamService.InitSteamClient();

            var matchDataService = container.Resolve<IMatchDataService>();

            var controller = new DataServiceController(steamService, matchDataService);
            var authData = await steamService.GetAuthTokenA();
            var authDataHex = HexUtil.ToHexString(authData.authToken);


            var steamId = authData.steamIdValue;
            var matchData = new MatchData()
            {
                ClientInfo = new ClientInfo()
                {
                    SteamId = steamId,
                    ClientName = nameof(SendGameDataTest)
                },
                MatchEndTime = DateTime.Now,
                MatchStartTime = DateTime.Now.AddMinutes(-2),
                ChamberDatas = new List<ChamberData>()
                {
                    new ChamberData()
                    {
                        ChamberNo = 0,
                        Duration = TimeSpan.FromSeconds(10),
                        PlayerDatas = new List<PlayerChamberData>()
                        {
                            new PlayerChamberData()
                            {
                                Name = "RealPlayerName",
                                Performance = new PlayerPerformance()
                                {
                                    HeadshotHits = 10,
                                    Hits = 10,
                                    Shots = 30,
                                }
                            }
                        }
                    }
                }
            };

            //using var compressedData = new MemoryStream();
            //using GZipStream compStream = new GZipStream(compressedData, CompressionLevel.Optimal);
            //await ServiceCore.ToJsonStreamAsync(compStream, matchData);
            //await compStream.FlushAsync();
            //var matchBinaryData = compressedData.ToArray();


            var result = await controller.SubmitMatchDataFunc(authDataHex, matchData);
        }

        [Fact]
        public async Task DevServerSendGameDataTest()
        {
            var container = new Container();
            container.Register<IDiagnosticService, NoOpDiagnosticService>(Reuse.Singleton);
            container.Register<ISteamService, SteamService>(Reuse.Singleton);
            container.Register<IMatchDataService, DynamoDBDataService>(Reuse.Singleton);

            var steamService = container.Resolve<ISteamService>();
            steamService.InitService(SteamAppId, SteamWebApiKey);
            steamService.InitSteamClient();

            var matchDataService = container.Resolve<IMatchDataService>();

            var controller = new DataServiceController(steamService, matchDataService);
            var authData = await steamService.GetAuthTokenA();
            var authDataHex = HexUtil.ToHexString(authData.authToken);


            var steamId = authData.steamIdValue;
            var matchData = new MatchData()
            {
                ClientInfo = new ClientInfo()
                {
                    SteamId = steamId,
                    ClientName = nameof(DevServerSendGameDataTest)
                },
                MatchEndTime = DateTime.Now,
                MatchStartTime = DateTime.Now.AddMinutes(-2),
                ChamberDatas = new List<ChamberData>()
                {
                    new ChamberData()
                    {
                        ChamberNo = 0,
                        Duration = TimeSpan.FromSeconds(10),
                        PlayerDatas = new List<PlayerChamberData>()
                        {
                            new PlayerChamberData()
                            {
                                Name = "RealPlayerName",
                                Performance = new PlayerPerformance()
                                {
                                    HeadshotHits = 10,
                                    Hits = 10,
                                    Shots = 30,
                                }
                            }
                        }
                    }
                }
            };

            using var compressedData = new MemoryStream();

            using (GZipStream compStream = new GZipStream(compressedData, CompressionLevel.Optimal, true))
            {
                await ServiceCore.ToJsonStreamAsync(compStream, matchData);
                compStream.Flush();
            }

            var matchBinaryData = compressedData.ToArray();

            using var httpClient = new HttpClient();
            var url = $"https://lambda-dev.ibotsota.net/api/DataService/SubmitMatchData?authDataHex={authDataHex}";

            var httpContent = new ByteArrayContent(matchBinaryData);
            httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            httpContent.Headers.ContentEncoding.Add("gzip");

            var response = await httpClient.PostAsync(url, httpContent);

            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }


        public async Task InitializeAsync()
        {
            var parameterClient = new AwsParameterStoreClient(RegionEndpoint.EUWest1);
            var steamAppId = await parameterClient.GetValueAsync("ibotsota-steamappid");
            var steamWebApiKey = await parameterClient.GetValueAsync("ibotsota-steamwebapikey");
            SteamAppId = uint.Parse(steamAppId);
            SteamWebApiKey = steamWebApiKey;

            var region = RegionEndpoint.EUWest1;

            AmazonAPIGatewayClient apiGateway = new AmazonAPIGatewayClient(new AmazonAPIGatewayConfig()
            {
                RegionEndpoint = region
            });

            var restApis = await apiGateway.GetRestApisAsync(new GetRestApisRequest());
            var devApi = restApis.Items.Single(item => item.Name.EndsWith("-dev"));
            var lambdaEndpointUrl = $"https://{devApi.Id}.execute-api.{region.SystemName}.amazonaws.com/dev";
            LambdaEndpointUrl = lambdaEndpointUrl;
        }

        public async Task DisposeAsync()
        {
        }
    }
    
 }