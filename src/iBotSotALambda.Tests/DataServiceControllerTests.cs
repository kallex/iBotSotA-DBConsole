using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon;
using Amazon.APIGateway;
using Amazon.APIGateway.Model;
using Amazon.DynamoDBv2.Model.Internal.MarshallTransformations;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using DataServiceCore;
using DryIoc;
using iBotSotALambda.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Xunit;

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
                container.Register<ISteamService, SteamService.SteamService>(Reuse.Singleton);
                var steamService = container.Resolve<ISteamService>();
                steamService.InitService(SteamAppId, SteamWebApiKey);
                steamService.InitSteamClient();

                var authData = await steamService.GetAuthTokenA();
                var authenticated = await steamService.ValidateAuthToken(authData.steamIdValue, authData.authToken);
                var dataServiceController = new DataServiceController();
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
            container.Register<ISteamService, SteamService.SteamService>(Reuse.Singleton);
            var steamService = container.Resolve<ISteamService>();
            steamService.InitService(SteamAppId, SteamWebApiKey);
            steamService.InitSteamClient();

            var authData = await steamService.GetAuthTokenA();
            var authDataHex = authData.authToken.ToHexString();
            var authenticated = await steamService.ValidateAuthTokenWeb(authDataHex);
            Assert.True(authenticated.authenticated);
        }


        [Fact]
        public async Task ServerAuthenticateTest()
        {
            var container = new Container();
            container.Register<ISteamService, SteamService.SteamService>(Reuse.Singleton);

            var steamService = container.Resolve<ISteamService>();
            steamService.InitService(SteamAppId, SteamWebApiKey);

            steamService.InitSteamClient();
            var authData = await steamService.GetAuthTokenA();

            var controller = new DataServiceController();
            var authDataHex = authData.authToken.ToHexString();
            //var result = (JsonResult) await controller.AuthTest(authData.steamIdValue, authDataHex);
            var result = (JsonResult) await controller.AuthTest(authDataHex);
            var expected = new JsonResult(new
            {
                statusMessage = "OK",
                authenticated = true
            });
            Assert.Equal(expected.Value, result.Value);
        }

        [Fact]
        public async Task SteamKeyValidationTest()
        {
            var container = new Container();
            container.Register<ISteamService, SteamService.SteamService>(Reuse.Singleton);

            var steamService = container.Resolve<ISteamService>();
            steamService.InitService(SteamAppId, SteamWebApiKey);

            steamService.InitSteamClient();
            var authData = await steamService.GetAuthTokenA();

            using var httpClient = new HttpClient();
            var authDataHex = authData.authToken.ToHexString();

            
            var url = $"https://partner.steam-api.com/ISteamUserAuth/AuthenticateUserTicket/v1/?key={SteamWebApiKey}&appid={SteamAppId}&ticket={authDataHex}";
            var response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task DevServerAuthenticateTest()
        {
            var container = new Container();
            container.Register<ISteamService, SteamService.SteamService>(Reuse.Singleton);

            var steamService = container.Resolve<ISteamService>();
            steamService.InitService(SteamAppId, SteamWebApiKey);

            steamService.InitSteamClient();
            var authData = await steamService.GetAuthTokenA();

            using var httpClient = new HttpClient();
            var authDataHex = authData.authToken.ToHexString();
            var url = $"{LambdaEndpointUrl}/api/DataService/AuthTestString?authDataHex={authDataHex}";
            var response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            Assert.StartsWith("Authenticated: True", content);
            Assert.EndsWith("Status: OK", content);
        }


        public async Task InitializeAsync()
        {
            var parameterClient = new AwsParameterStoreClient(RegionEndpoint.EUWest1);
            var steamAppId = await parameterClient.GetValueAsync("ibotsota-steamappid");
            var steamWebApiKey = await parameterClient.GetValueAsync("	ibotsota-steamwebapikey");
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