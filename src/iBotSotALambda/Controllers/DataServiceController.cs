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
            var now = DateTime.Now;
            return $"Now: {now} Arg: {arg}";
        }

        [HttpGet("{arg}")]
        public async Task<string> TestXGetAsync(int arg)
        {
            var now = DateTime.Now;
            return $"Now: {now} Arg: {arg}";
        }


        [HttpGet("{arg}")]
        public async Task TestGetAsyncStream(int arg)
        {
            var now = DateTime.Now;
            var result = $"{nameof(TestGetAsyncStream)} Now: {now} Arg: {arg}";
            var data = Encoding.UTF8.GetBytes(result);
            await Response.Body.WriteAsync(data);
            //Response.StatusCode = 200;
        }


        [HttpGet("{arg}")]
        public async Task<string> DiagTest(string arg)
        {
            string statusMessage = "";
            try
            {
                /*
                var rules = DryIoc.Rules.Default
                    .With(DryIoc.FactoryMethod.ConstructorWithResolvableArguments)
                    .WithFactorySelector(DryIoc.Rules.SelectLastRegisteredFactory())
                    .WithTrackingDisposableTransients();

                this.Container = new DryIoc.Container(rules);
                Container.Register<ISteamService, SteamService.SteamService>(Reuse.Singleton);


                var steamService = Container.Resolve<ISteamService>();
                steamService.InitService(Startup.SteamAppId, Startup.SteamWebApiKey);
                authenticated = await steamService.ValidateAuthTokenWeb(arg);
                statusMessage = "OK";
                */
                if (!arg.StartsWith("nop"))
                {
                    using var httpClient = new HttpClient();
                    var fullUrl = $"https://{arg}";
                    var response = await httpClient.GetAsync(fullUrl);
                    var statusCode = response.StatusCode;
                    statusMessage = $"Response status for {fullUrl} was {statusCode}";
                }
            }
            catch (Exception ex)
            {
                statusMessage = ex.ToString();
            }

            var result = $"Diagtest {DateTime.Now}: Status: {statusMessage}";
            return result;
        }


        [HttpGet("{authDataHex}")]
        public async Task<string> AuthTestNamedPartial(string authDataHex)
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
                //var steamService = (ISteamService) new SteamService.SteamService();
                steamService.InitService(Startup.SteamAppId, Startup.SteamWebApiKey);
                //authenticated = await steamService.ValidateAuthTokenWeb(authDataHex);
                statusMessage = "OK (not)";
            }
            catch (Exception ex)
            {
                statusMessage = ex.ToString();
            }

            var result = $"AuthData: {authDataHex} Authenticated: {authenticated} - Status: {statusMessage}";
            return result;
        }



        [HttpGet]
        public async Task<string> AuthTestString(string authDataHex)
        {
            string statusMessage = "";
            (bool authenticated, ulong steamid, ulong ownersteamid, bool vacbanned, bool publishedbanned) authenticated = default;
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

            var result = $"Authenticated: {authenticated.authenticated} - SteamID: {authenticated.steamid} - Status: {statusMessage}";
            return result;
        }


        [HttpGet]
        public async Task<ActionResult> AuthTest([FromQuery] string authDataHex)
        {
            string statusMessage = "";
            (bool authenticated, ulong steamid, ulong ownersteamid, bool vacbanned, bool publishedbanned) authenticated = default;
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
                authenticated = authenticated.authenticated,
                steamid = authenticated.steamid
            });
        }
    }
}