using System;
using System.Threading.Tasks;
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
    public class DataServiceControllerTests
    {
        private const uint SteamAppId = 1518060;
        [Fact]
        public async Task SelfAuthenticationTest()
        {
            try
            {
                var container = new Container();
                container.Register<ISteamService, SteamService.SteamService>(Reuse.Singleton);
                var steamService = container.Resolve<ISteamService>();
                steamService.InitService(SteamAppId);

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
        public async Task ServerAuthenticateTest()
        {
            try
            {
                var container = new Container();
                container.Register<ISteamService, SteamService.SteamService>(Reuse.Singleton);
                
                var steamService = container.Resolve<ISteamService>();
                steamService.InitService(SteamAppId);

                var authData = await steamService.GetAuthTokenA();

                var controller = new DataServiceController();
                var authDataHex = authData.authToken.ToHexString();
                var result = (JsonResult) await controller.AuthTest(authData.steamIdValue, authDataHex);
                var expected = new JsonResult(new
                {
                    statusMessage = "OK",
                    authenticated = true
                });
                Assert.Equal(expected, result);
            }
            catch (Exception ex)
            {

            }
        }


    }
}