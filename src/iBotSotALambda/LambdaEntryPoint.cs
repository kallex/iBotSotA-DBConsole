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
using SteamServices;

namespace iBotSotALambda
{
    /// <summary>
    /// This class extends from APIGatewayProxyFunction which contains the method FunctionHandlerAsync which is the 
    /// actual Lambda function entry point. The Lambda handler field should be set to
    /// 
    /// iBotSotALambda::iBotSotALambda.LambdaEntryPoint::FunctionHandlerAsync
    /// </summary>
    public class LambdaEntryPoint :

        // The base class must be set to match the AWS service invoking the Lambda function. If not Amazon.Lambda.AspNetCoreServer
        // will fail to convert the incoming request correctly into a valid ASP.NET Core request.
        //
        // API Gateway REST API                         -> Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
        // API Gateway HTTP API payload version 1.0     -> Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
        // API Gateway HTTP API payload version 2.0     -> Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction
        // Application Load Balancer                    -> Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction
        // 
        // Note: When using the AWS::Serverless::Function resource with an event type of "HttpApi" then payload version 2.0
        // will be the default and you must make Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction the base class.

        Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    {
        /// <summary>
        /// The builder has configuration, logging and Amazon API Gateway already configured. The startup class
        /// needs to be configured in this method using the UseStartup<>() method.
        /// </summary>
        /// <param name="builder"></param>
        protected override void Init(IWebHostBuilder builder)
        {
            builder
                .UseStartup<Startup>();
        }

        /// <summary>
        /// Use this override to customize the services registered with the IHostBuilder. 
        /// 
        /// It is recommended not to call ConfigureWebHostDefaults to configure the IWebHostBuilder inside this method.
        /// Instead customize the IWebHostBuilder in the Init(IWebHostBuilder) overload.
        /// </summary>
        /// <param name="builder"></param>
        protected override void Init(IHostBuilder builder)
        {
            builder.UseServiceProviderFactory(new DryIocServiceProviderFactory())
                .ConfigureContainer<Container>((hostContext, container) =>
                {
                    container.Register<ISteamService, SteamService>(Reuse.Singleton);
                    container.Register<IDiagnosticService, AWSXRayService>(Reuse.Singleton);
                    container.Register<IMatchDataService, DynamoDBDataService>(Reuse.Singleton);

                    var steamService = container.Resolve<ISteamService>();
                    var parameterClient = new AwsParameterStoreClient(RegionEndpoint.EUWest1);
                    uint SteamAppId = default;
                    string SteamWebApiKey = default;
                    var asyncTask = Task.Run(async () =>
                    {
                        var steamAppId = await parameterClient.GetValueAsync("ibotsota-steamappid");
                        var steamWebApiKey = await parameterClient.GetValueAsync("	ibotsota-steamwebapikey");
                        SteamAppId = uint.Parse(steamAppId);
                        SteamWebApiKey = steamWebApiKey;
                    });

                    asyncTask.Wait();
                    steamService.InitService(SteamAppId, SteamWebApiKey);
                });
        }
    }
}
