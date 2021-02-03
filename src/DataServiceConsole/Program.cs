using System;
using System.CommandLine;
using System.Threading.Tasks;
using AWSDataService;
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

            Console.WriteLine("Hello World!");
            return 0;
        }
    }
}
