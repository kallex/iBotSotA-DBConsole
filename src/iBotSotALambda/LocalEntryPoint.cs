using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using AWSDataServices;
using Services;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace iBotSotALambda
{
    /// <summary>
    /// The Main function can be used to run the ASP.NET Core application locally using the Kestrel webserver.
    /// </summary>
    public class LocalEntryPoint
    {
        public static void Main(string[] args)
        {
            Startup.IsRunningInLambda = false;
            var builder = CreateHostBuilder(args);
            var app = builder.Build();
            app.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseServiceProviderFactory(new DryIocServiceProviderFactory())
                .ConfigureContainer<Container>((hostContext, container) =>
                {
                    container.Register<IDiagnosticService, NoOpDiagnosticService>(Reuse.Singleton);
                    container.Register<ISteamService, SteamServices.SteamService>(Reuse.Singleton);
                    container.Register<IMatchDataService, DynamoDBDataService>(Reuse.Singleton);

                    var steamService = container.Resolve<ISteamService>();
                    var parameterClient = new AwsParameterStoreClient(RegionEndpoint.EUWest1);
                    uint SteamAppId = default;
                    string SteamWebApiKey = default;
                    var asyncTask = Task.Run(async () =>
                    {
                        var steamAppId = await parameterClient.GetValueAsync("ibotsota-steamappid");
                        var steamWebApiKey = await parameterClient.GetValueAsync("ibotsota-steamwebapikey");
                        SteamAppId = uint.Parse(steamAppId);
                        SteamWebApiKey = steamWebApiKey;
                    });

                    asyncTask.Wait();
                    steamService.InitService(SteamAppId, SteamWebApiKey);
                });
    }
}
