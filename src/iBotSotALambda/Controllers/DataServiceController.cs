using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Services;
using DryIoc;
using Microsoft.AspNetCore.Mvc;

namespace iBotSotALambda.Controllers
{
    [Route("api/[controller]/[action]")]
    public class DataServiceController : ControllerBase
    {
        public DataServiceController(ISteamService steamService)
        {
            SteamService = steamService;
        }

        public ISteamService SteamService { get; }

        public Container Container { get; set; }

        [HttpPost]
        public async Task<ActionResult> SubmitMatchData([FromQuery] string authDataHex, [FromBody] byte[] matchData)
        {
            var authResult = await SteamService.ValidateAuthTokenWeb(authDataHex);

            //var container = new Container();
            //container.Register<ISteamService, SteamService.SteamService>(Reuse.Singleton);
            //var steamService = container.Resolve<ISteamService>();

            //steamService.InitService();
            //var authToken = steamService.GetAuthTokenA();
            return new AcceptedResult();
        }


        [HttpGet]
        public async Task<ActionResult> GetSteamAuthentication([FromQuery] string authDataHex)
        {
            var authResult = await SteamService.ValidateAuthTokenWeb(authDataHex);

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