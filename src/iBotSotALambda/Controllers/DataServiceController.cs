using System;
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
        public async Task<string> AuthTestArgEmpty(string arg)
        {
            var now = DateTime.Now;
            return $"Now: {now} Arg: {arg}";
        }


        [HttpGet("{arg}")]
        public async Task<string> AuthTestArg(string arg)
        {
            string statusMessage = "";
            bool authenticated = false;
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
            }
            catch (Exception ex)
            {
                statusMessage = ex.ToString();
            }

            var result = $"Authenticated: {authenticated} - Status: {statusMessage}";
            return result;
        }


        [HttpGet("{authDataHex}")]
        public async Task<string> AuthTestNamedPartial(string authDataHex)
        {
            string statusMessage = "";
            bool authenticated = false;
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
                authenticated = await steamService.ValidateAuthTokenWeb(authDataHex);
                statusMessage = "OK";
                */
            }
            catch (Exception ex)
            {
                statusMessage = "Error catched.";
            }

            var result = $"Authenticated: {authenticated} - Status: {statusMessage}";
            return result;
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