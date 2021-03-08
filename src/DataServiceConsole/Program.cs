using System;
using System.CommandLine;
using System.Threading.Tasks;
using AWSDataServices;
using Services;
using DryIoc;

namespace DataServiceConsole
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            using var container = new Container();

            container.Register<DynamoDBDataService>(Reuse.Singleton);
            container.Register<TimestreamDataService>(Reuse.Singleton);
            container.Register<ISteamService, SteamServices.SteamService>(Reuse.Singleton);

            var steamService = container.Resolve<ISteamService>();

            Console.WriteLine("Hello World!");
            return 0;
        }
    }
}
