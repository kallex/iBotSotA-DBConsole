using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DataServiceCore;
using Services;
using DryIoc;
using Microsoft.AspNetCore.Mvc;

namespace iBotSotALambda.Controllers
{
    [Route("api/[controller]/[action]")]
    public class DataServiceController : ControllerBase
    {
        public DataServiceController(ISteamService steamService, IMatchDataService matchDataService)
        {
            SteamService = steamService;
            MatchDataService = matchDataService;
        }

        public IMatchDataService MatchDataService { get; }

        public ISteamService SteamService { get; }

        public Container Container { get; set; }

        [HttpPost]
        public async Task<ActionResult> SubmitMatchData([FromQuery] string authDataHex)
        {
            //var compressedStream = Request.Body;
            //var decompStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            //var matchData = await ServiceCore.FromJsonAsync<MatchData>(decompStream);

            var jsonStream = Request.Body;
            var matchData = await ServiceCore.FromJsonAsync<MatchData>(jsonStream);

            //var container = new Container();
            //container.Register<ISteamService, SteamService.SteamService>(Reuse.Singleton);
            //var steamService = container.Resolve<ISteamService>();

            //steamService.InitService();
            //var authToken = steamService.GetAuthTokenA();
            var result = await SubmitMatchDataFunc(authDataHex, matchData);
            return result;
        }

        public async Task<ActionResult> SubmitMatchDataFunc(string authDataHex, MatchData matchData)
        {
            var authResult = await SteamService.ValidateAuthTokenWeb(authDataHex);
            if (!authResult.isAuthenticated || authResult.publisherBanned || authResult.vacBanned)
                throw new UnauthorizedAccessException();
            matchData.ClientInfo.SteamId = authResult.steamId;
            await MatchDataService.StoreMatchData(matchData);
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