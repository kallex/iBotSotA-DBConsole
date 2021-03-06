using System;
using System.Threading.Tasks;
using DataServiceCore;
using DryIoc;
using Microsoft.AspNetCore.Mvc;

namespace iBotSotALambda.Controllers
{
    [Route("api/[controller]/[action]")]
    public class DataServiceController : ControllerBase
    {
        public DataServiceController()
        {
        }

        public Container Container { get; set; }

        [HttpPost]
        public void SubmitMatchData([FromBody] byte[] matchData)
        {
            //var container = new Container();
            //container.Register<ISteamService, SteamService.SteamService>(Reuse.Singleton);
            //var steamService = container.Resolve<ISteamService>();

            //steamService.InitService();
            //var authToken = steamService.GetAuthTokenA();
        }

        [HttpGet("{arg}")]
        public string TestGet(int arg)
        {
            return $"arg: {arg}";
        }

        [HttpGet]
        public async Task<string> AuthTestString(string authDataHex)
        {
            string statusMessage = "";
            bool authenticated = false;
            try
            {
                var rules = DryIoc.Rules.Default
                    .With(DryIoc.FactoryMethod.ConstructorWithResolvableArguments)
                    .WithFactorySelector(DryIoc.Rules.SelectLastRegisteredFactory())
                    .WithTrackingDisposableTransients();

                this.Container = new DryIoc.Container(rules);
                Container.Register<ISteamService, SteamService.SteamService>(Reuse.Singleton);


                var steamService = Container.Resolve<ISteamService>();
                steamService.InitService(Startup.SteamAppId, Startup.SteamWebApiKey);
                authenticated = await steamService.ValidateAuthTokenWeb(authDataHex);
                statusMessage = "OK";
            }
            catch (Exception ex)
            {
                statusMessage = ex.ToString();
            }

            var result = $"Authenticated: {authenticated} - Status: {statusMessage}";
            return result;
        }


        [HttpGet]
        public async Task<ActionResult> AuthTest([FromQuery] string authDataHex)
        {
            string statusMessage = "";
            bool authenticated = false;
            try
            {
                var rules = DryIoc.Rules.Default
                    .With(DryIoc.FactoryMethod.ConstructorWithResolvableArguments)
                    .WithFactorySelector(DryIoc.Rules.SelectLastRegisteredFactory())
                    .WithTrackingDisposableTransients();

                this.Container = new DryIoc.Container(rules);
                Container.Register<ISteamService, SteamService.SteamService>(Reuse.Singleton);


                var steamService = Container.Resolve<ISteamService>();
                steamService.InitService(Startup.SteamAppId, Startup.SteamWebApiKey);
                authenticated = await steamService.ValidateAuthTokenWeb(authDataHex);
                statusMessage = "OK";
            }
            catch (Exception ex)
            {
                statusMessage = ex.ToString();
            }

            return new JsonResult(new
            {
                statusMessage = statusMessage,
                authenticated = authenticated
            });
        }
    }
}