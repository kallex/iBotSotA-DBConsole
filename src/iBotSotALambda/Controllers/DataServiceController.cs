using System;
using System.Net.Http;
using System.Text;
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
        public void SubmitMatchData([FromQuery] string authDataHex, [FromBody] byte[] matchData)
        {
            //var container = new Container();
            //container.Register<ISteamService, SteamService.SteamService>(Reuse.Singleton);
            //var steamService = container.Resolve<ISteamService>();

            //steamService.InitService();
            //var authToken = steamService.GetAuthTokenA();
        }


        [HttpGet]
        public async Task<ActionResult> GetSteamAuthentication([FromQuery] string authDataHex)
        {
            var rules = DryIoc.Rules.Default
                .With(DryIoc.FactoryMethod.ConstructorWithResolvableArguments)
                .WithFactorySelector(DryIoc.Rules.SelectLastRegisteredFactory())
                .WithTrackingDisposableTransients();

            this.Container = new DryIoc.Container(rules);
            Container.Register<ISteamService, SteamService.SteamService>(Reuse.Singleton);


            var steamService = Container.Resolve<ISteamService>();
            steamService.InitService(Startup.SteamAppId, Startup.SteamWebApiKey);
            var authResult = await steamService.ValidateAuthTokenWeb(authDataHex);

            return new JsonResult(new
            {
                authResult.isAuthenticated,
                authResult.steamId,
                authResult.ownerSteamId,
                authResult.vacBanned,
                authResult.publisherBanned
            });
        }
    }
}